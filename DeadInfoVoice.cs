using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json;

namespace DeadInfoVoice;

public class DeadInfoVoice : BasePlugin
{
    public override string ModuleName => "DeadInfoVoice";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "BiKsu";
    public override string ModuleDescription => "Players can talk for a few seconds after death.";

    private readonly HashSet<ulong> AllowedDeadPlayers = new();

    private Dictionary<string, string> Lang = new();

    private PluginConfig Config = new();

    private string ConfigPath =>
        Path.Combine(
            Server.GameDirectory,
            "csgo",
            "addons",
            "counterstrikesharp",
            "configs",
            "plugins",
            "DeadInfoVoice",
            "config.json"
        );

    private string LangPath =>
        Path.Combine(
            Server.GameDirectory,
            "csgo",
            "addons",
            "counterstrikesharp",
            "configs",
            "plugins",
            "DeadInfoVoice",
            "lang",
            $"{Config.Language}.json"
        );

    public override void Load(bool hotReload)
    {
        CreateConfig();
        LoadConfig();

        CreateLanguageFiles();
        LoadTranslations();

        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterListener<Listeners.OnTick>(OnTick);

        Server.ExecuteCommand("sv_deadtalk 1");
    }

    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot)
            return HookResult.Continue;

        ulong steamId = player.SteamID;

        AllowedDeadPlayers.Add(steamId);

        player.PrintToChat(
            Translate("dead_info_start")
                .Replace("{TIME}", Config.VoiceTime.ToString())
        );

        AddTimer(Config.VoiceTime, () =>
        {
            AllowedDeadPlayers.Remove(steamId);

            if (player.IsValid)
            {
                player.PrintToChat(
                    Translate("dead_info_end")
                );
            }
        });

        return HookResult.Continue;
    }

    private void OnTick()
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null || !player.IsValid || player.IsBot)
                continue;

            if (player.PawnIsAlive)
                continue;

            bool canSpeak = AllowedDeadPlayers.Contains(player.SteamID);

            player.VoiceFlags = canSpeak
                ? VoiceFlags.Normal
                : VoiceFlags.Muted;
        }
    }

    private void CreateConfig()
    {
        var configDir = Path.GetDirectoryName(ConfigPath);

        if (!Directory.Exists(configDir))
            Directory.CreateDirectory(configDir!);

        if (File.Exists(ConfigPath))
            return;

        var json = JsonSerializer.Serialize(
            new PluginConfig(),
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(ConfigPath, json);
    }

    private void LoadConfig()
    {
        if (!File.Exists(ConfigPath))
            return;

        var json = File.ReadAllText(ConfigPath);

        Config = JsonSerializer.Deserialize<PluginConfig>(json)
                 ?? new PluginConfig();
    }

    private void CreateLanguageFiles()
    {
        var langDir = Path.Combine(
            Server.GameDirectory,
            "csgo",
            "addons",
            "counterstrikesharp",
            "configs",
            "plugins",
            "DeadInfoVoice",
            "lang"
        );

        if (!Directory.Exists(langDir))
            Directory.CreateDirectory(langDir);

        CreateLanguageFile(
            Path.Combine(langDir, "pl.json"),
            new Dictionary<string, string>
            {
                ["dead_info_start"] = "{GREEN}● {DEFAULT}Masz {LIGHTGREEN}{TIME} sekund{DEFAULT} na przekazanie informacji drużynie.",
                ["dead_info_end"] = "{RED}● {DEFAULT}Czas na przekazanie informacji minął."
            });

        CreateLanguageFile(
            Path.Combine(langDir, "en.json"),
            new Dictionary<string, string>
            {
                ["dead_info_start"] = "{GREEN}● {DEFAULT}You have {LIGHTGREEN}{TIME} seconds{DEFAULT} to give information to your team.",
                ["dead_info_end"] = "{RED}● {DEFAULT}Your info time has expired."
            });
    }

    private void CreateLanguageFile(
        string path,
        Dictionary<string, string> content)
    {
        if (File.Exists(path))
            return;

        var json = JsonSerializer.Serialize(
            content,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(path, json);
    }

    private void LoadTranslations()
    {
        if (!File.Exists(LangPath))
            return;

        var json = File.ReadAllText(LangPath);

        Lang = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
               ?? new Dictionary<string, string>();
    }

    private string Translate(string key)
    {
        if (!Lang.TryGetValue(key, out var value))
            return key;

        return value
            .Replace("{DEFAULT}", ChatColors.Default.ToString())
            .Replace("{GREEN}", ChatColors.Green.ToString())
            .Replace("{LIME}", ChatColors.Lime.ToString())
            .Replace("{RED}", ChatColors.Red.ToString());
    }

    public override void Unload(bool hotReload)
    {
        AllowedDeadPlayers.Clear();

        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null || !player.IsValid)
                continue;

            player.VoiceFlags = VoiceFlags.Normal;
        }
    }
}

public class PluginConfig
{
    public float VoiceTime { get; set; } = 5.0f;

    public string Language { get; set; } = "pl";
}