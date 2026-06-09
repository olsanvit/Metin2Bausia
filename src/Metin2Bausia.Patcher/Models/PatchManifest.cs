using Newtonsoft.Json;

namespace Metin2Bausia.Patcher.Models;

public class PatchManifest
{
    [JsonProperty("version")]     public string Version     { get; set; } = "";
    [JsonProperty("date")]        public DateTime Date      { get; set; }
    [JsonProperty("serverName")]  public string ServerName  { get; set; } = "";
    [JsonProperty("serverIp")]    public string ServerIp    { get; set; } = "";
    [JsonProperty("serverPort")]  public int ServerPort     { get; set; }
    [JsonProperty("baseUrl")]     public string BaseUrl     { get; set; } = "";
    [JsonProperty("news")]        public string News        { get; set; } = "";
    [JsonProperty("websiteUrl")]  public string WebsiteUrl  { get; set; } = "";
    [JsonProperty("registerUrl")] public string RegisterUrl { get; set; } = "";
    [JsonProperty("files")]       public List<PatchFile> Files { get; set; } = new();
    [JsonProperty("launcher")]    public LauncherConfig Launcher { get; set; } = new();
}

public class PatchFile
{
    [JsonProperty("path")]     public string Path     { get; set; } = "";
    [JsonProperty("sha256")]   public string Sha256   { get; set; } = "";
    [JsonProperty("size")]     public long Size       { get; set; }
    [JsonProperty("url")]      public string? Url     { get; set; }
    [JsonProperty("required")] public bool Required   { get; set; } = true;
}

public class LauncherConfig
{
    [JsonProperty("exe")]  public string Exe  { get; set; } = "metin2.exe";
    [JsonProperty("args")] public string Args { get; set; } = "";
}
