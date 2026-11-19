using AutoMapper;
using ConsultaProcessosApi.Data.Dtos;
using ConsultaProcessosApi.Datas;
using ConsultaProcessosApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsultaProcessosApi.Service;

public class ConsultaService
{
    private ConsultaContext _context;
    private HttpClient _httpClient;
    private IMapper _mapper;


    public ConsultaService(ConsultaContext context, HttpClient httpClient, IMapper mapper)
    {
        _context = context;
        _httpClient = httpClient;
        _mapper = mapper;
    }

    private static readonly Dictionary<string, string> UfPorCodigo = new()
    {
        ["01"] = "ac",
        ["02"] = "al",
        ["03"] = "ap",
        ["04"] = "am",
        ["05"] = "ba",
        ["06"] = "ce",
        ["07"] = "dft",
        ["08"] = "es",
        ["09"] = "go",
        ["10"] = "ma",
        ["11"] = "mt",
        ["12"] = "ms",
        ["13"] = "mg",
        ["14"] = "pa",
        ["15"] = "pb",
        ["16"] = "pr",
        ["17"] = "pe",
        ["18"] = "pi",
        ["19"] = "rj",
        ["20"] = "rn",
        ["21"] = "rs",
        ["22"] = "ro",
        ["23"] = "rr",
        ["24"] = "sc",
        ["25"] = "se",
        ["26"] = "sp",
        ["27"] = "to"
    };

    private static readonly Dictionary<string, string> NomeEstado = new()
    {
        ["ac"] = "Acre",
        ["al"] = "Alagoas",
        ["ap"] = "Amapá",
        ["am"] = "Amazonas",
        ["ba"] = "Bahia",
        ["ce"] = "Ceará",
        ["dft"] = "Distrito Federal e Territórios",
        ["es"] = "Espírito Santo",
        ["go"] = "Goiás",
        ["ma"] = "Maranhão",
        ["mt"] = "Mato Grosso",
        ["ms"] = "Mato Grosso do Sul",
        ["mg"] = "Minas Gerais",
        ["pa"] = "Pará",
        ["pb"] = "Paraíba",
        ["pr"] = "Paraná",
        ["pe"] = "Pernambuco",
        ["pi"] = "Piauí",
        ["rj"] = "Rio de Janeiro",
        ["rn"] = "Rio Grande do Norte",
        ["rs"] = "Rio Grande do Sul",
        ["ro"] = "Rondônia",
        ["rr"] = "Roraima",
        ["sc"] = "Santa Catarina",
        ["se"] = "Sergipe",
        ["sp"] = "São Paulo",
        ["to"] = "Tocantins"
    };

    public async Task<List<ReadConsultaDto>> ConsultarEResgistrar(string numeroProcesso)
    {
        // 1. Descobrir o alias do tribunal e limpar o número
        var alias = ObterAlias(numeroProcesso);
        var numero = NormalizarProcesso(numeroProcesso);

        if (alias == null || numero == null)
        {
            throw new ArgumentException("Número de processo inválido ou tribunal não suportado.");
        }

        // 2. Montar o corpo da requisição
        var json = JsonSerializer.Serialize(new
        {
            query = new { match = new { numeroProcesso = numero } }
        });

        var corpo = new StringContent(json, Encoding.UTF8, "application/json");

        // 3. Chamar a API
        var resposta = await _httpClient.PostAsync($"api_publica_{alias}/_search", corpo);
        var jsonResp = await resposta.Content.ReadAsStringAsync();
        resposta.EnsureSuccessStatusCode();

        // 4. Converter o JSON nas classes
        var resultado = JsonSerializer.Deserialize<DatajudResponse>(jsonResp);

        // 5. Buscar o tribunal no banco
        var tribunal = _context.Tribunais.FirstOrDefault(t => t.Sigla.ToLower() == alias);

        if (tribunal == null)
        {
            tribunal = new Tribunais
            {
                Sigla = alias.ToUpper(),
                Nome = ObterNomeTribunal(alias)
            };

            _context.Tribunais.Add(tribunal);
            await _context.SaveChangesAsync();
        ;
        }

        // 6. Montar a lista de consultas, um processo por vez
        var consultas = new List<ConsultaProcessos>();

        foreach (var hit in resultado.hitsOuver.hits)
        {
            var processo = hit.source;

            // Classe: se não vier, usa um texto padrão
            string classe;
            if (processo.classe == null)
            {
                classe = "Não informada";
            }
            else
            {
                classe = processo.classe.nome;
            }

            // Assuntos: se não vier, usa um texto padrão
            string assunto;
            if (processo.assuntos == null)
            {
                assunto = "Não informado";
            }
            else
            {
                var nomes = new List<string>();
                foreach (var a in processo.assuntos)
                {
                    nomes.Add(a.nome);
                }
                assunto = string.Join(", ", nomes);
            }

            var consulta = new ConsultaProcessos
            {
                NumeroProcesso = processo.numeroProcesso,
                ClasseProcesso = classe,
                AssuntoProcesso = assunto,
                TribunaisId = tribunal.Id,
                Tribunais = tribunal
            };

            consultas.Add(consulta);
        }

        // 7. Salvar no banco
        _context.Consultas.AddRange(consultas);
        await _context.SaveChangesAsync();

        // 8. Converter para o DTO e devolver
        var respostaDto = _mapper.Map<List<ReadConsultaDto>>(consultas);
        return respostaDto;
    }

    public string? NormalizarProcesso(string numeroProcesso)
    {
        var numero = new string(numeroProcesso.Where(char.IsDigit).ToArray());

        if (numero.Length != 20)
        {
            return null;
        }

        return numero;
    }

    private string? ObterAlias(string processo)
    {
        var numero = NormalizarProcesso(processo);
        if (numero == null) return null;

        var justica = numero[13]; // Ramo da justiça
        var tribunal = numero.Substring(14, 2); // tribunal

        return justica switch
        {
            '4' => $"trf{int.Parse(tribunal)}",
            '5' => $"trt{int.Parse(tribunal)}",
            '8' => UfPorCodigo.TryGetValue(tribunal, out var uf) ? $"tj{uf}" : null, //  aqui procuramos de uma fomra segura o aluas, com TryGetValue se rencontrarr armazena e nos retorna
            _ => null      // caso não exista nenhuma regra para o numero encontrado sempre vai cair em Null                    
        };
    }

    private string ObterNomeTribunal(string alias)
    {
        if (alias.StartsWith("trf"))
        {
            var numero = alias.Substring(3);
            return $"Tribunal Regional Federal da {numero}ª Região";// "trf1" -> "1"
        }

        if (alias.StartsWith("trt"))
        {
            var numero = alias.Substring(3);
            return $"Tribunal Regional do Trabalho da {numero}ª Região";// "trt23" -> "23"
        }

        if (alias.StartsWith("tj"))
        {
            var uf = alias.Substring(2);
            if(NomeEstado.TryGetValue(uf, out var estado))
            {
                return $"Tribunal de Justiça - {estado}"; // "tjsp" -> "sp"
            }
        }

        return alias.ToUpper(); // se nada bateu, usa a sigla
    }


    public record DatajudResponse([property: JsonPropertyName("hits")] HitsOuver hitsOuver);
    public record HitsOuver([property: JsonPropertyName("hits")] List<Hit> hits);
    public record Hit([property: JsonPropertyName("_source")] DatajudProcesso source);
    public record DatajudProcesso(
        [property: JsonPropertyName("numeroProcesso")] string numeroProcesso,
        [property: JsonPropertyName("classe")] CodigoNome classe,
        [property: JsonPropertyName("assuntos")] List<CodigoNome> assuntos,
        [property: JsonPropertyName("tribunal")] string tribunal);

    public record CodigoNome(
        [property: JsonPropertyName("codigo")] int codigo,
        [property: JsonPropertyName("nome")] string nome
        );
}