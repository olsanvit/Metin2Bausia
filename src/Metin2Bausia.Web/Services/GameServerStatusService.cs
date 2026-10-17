using System.Net.Sockets;

namespace Metin2Bausia.Web.Services;

public class GameEndpoint
{
    public string Name { get; set; } = "";
    public string Host { get; set; } = "";
    public int Port { get; set; }
}

public enum GameServerState { NotDeployed, Running, Partial, Down }

public record GameEndpointStatus(string Name, int Port, bool Open);
public record GameServerStatus(GameServerState State, IReadOnlyList<GameEndpointStatus> Endpoints);

/// <summary>
/// Stav herního serveru podle TCP spojení na porty auth/game (rozhodnutí Vítka 17. 10.).
/// Web nemá přístup k Dockeru, ping portu stačí na „běží / neběží“. Adresy jsou jen v konfiguraci
/// (GameServer:Endpoints) — repo je veřejné, adresa QNAPu do něj nepatří; bez konfigurace = nenasazeno.
/// </summary>
public class GameServerStatusService(IConfiguration config)
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1.5);

    public async Task<GameServerStatus> GetAsync(CancellationToken ct = default)
    {
        var endpoints = config.GetSection("GameServer:Endpoints").Get<List<GameEndpoint>>()?
            .Where(e => !string.IsNullOrWhiteSpace(e.Host) && e.Port is > 0 and < 65536).ToList() ?? [];
        if (endpoints.Count == 0) return new(GameServerState.NotDeployed, []);

        var results = await Task.WhenAll(endpoints.Select(async e =>
            new GameEndpointStatus(e.Name, e.Port, await IsOpenAsync(e.Host, e.Port, ct))));
        return new(Evaluate(results), results);
    }

    public static GameServerState Evaluate(IReadOnlyCollection<GameEndpointStatus> results) =>
        results.Count == 0 ? GameServerState.NotDeployed
        : results.All(r => r.Open) ? GameServerState.Running
        : results.Any(r => r.Open) ? GameServerState.Partial
        : GameServerState.Down;

    private static async Task<bool> IsOpenAsync(string host, int port, CancellationToken ct)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(Timeout);
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(host, port, cts.Token);
            return true;
        }
        catch { return false; }   // timeout, odmítnuté spojení i neznámý host = port nedostupný
    }
}
