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
    private IConfiguration configuration;


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

    public ConsultaService(ConsultaContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api-publica.datajud.cnj.jus.br");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"APIKey {configuration["AuthorizationKey"]}");
    }

    public async Task<List<ConsultaProcessos>> ConsultarEResgistrar(string numeroProcesso)
    {

        var alias = ObterAlias(numeroProcesso);
        var numero = NormalizarProcesso(numeroProcesso);

        var url = $"{_httpClient.BaseAddress}/api_publica_{alias}/_search";

        var corpo = new StringContent(
            $$"""{"query":{"match":{"numeroProcesso":"{{numero}}"}}}""",
            Encoding.UTF8,
            "application/json"
            );

        var resposta = await _httpClient.PostAsync(url, corpo);
        var json = await resposta.Content.ReadAsStringAsync();
        resposta.EnsureSuccessStatusCode();

        var resultado = JsonSerializer.Deserialize<DatajudResponse>(json);

        return resultado;
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

    public string? ObterAlias(string processo)
    {
        var numero = NormalizarProcesso(processo);

        var justica = numero[13]; // Ramo da justiça
        var tribunal = numero.Substring(14, 2); // tribunal

        return j switch
        {
            '4' => $"trf{int.Parse(tribunal)}",
            '5' => $"trt{int.Parse(tribunal)}",
            '8' => UfPorCodigo.TryGetValue(tribunal, out var uf) ? $"tj{uf}" : null, //  aqui procuramos de uma fomra segura o aluas, com TryGetValue se rencontrarr armazena e nos retorna
            _ => null      // caso não exista nenhuma regra para o numero encontrado sempre vai cair em Null                    
        };
    }


    public record DatajudResponse([property: JsonPropertyName("hits")] HitsOuver hitsOuver);
    public record HitsOuver([property: JsonPropertyName("hits")] List<Hit> hits);
    public record Hit([property: JsonPropertyName("_source")] DatajudProcesso source);
    public record DatajudProcesso(
        [property: JsonPropertyName("numeroProcesso")] string numeroProcesso,
        [property: JsonPropertyName("classeProcessual")] string classeProcessual,
        [property: JsonPropertyName("assunto")] string assunto,
        [property: JsonPropertyName("dataDistribuicao")] string dataDistribuicao,
        [property: JsonPropertyName("foro")] string foro,
        [property: JsonPropertyName("vara")] string vara,
        [property: JsonPropertyName("juiz")] string juiz
        );

    public record CodigoNome(
        [property: JsonPropertyName("codigo")] int codigo,
        [property: JsonPropertyName("nome")] string nome
        );
}
