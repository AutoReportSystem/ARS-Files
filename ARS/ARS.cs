using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace ARS;

[BepInIncompatibility("industry.resurgencev2")]
[BepInPlugin("industry.autoreportsys", "Automatic Reporting System", "1.0.0")]
internal class ARS : BaseUnityPlugin
{
    #region Main

    private static readonly HttpClient client = new();

    public static readonly List<Player> PlayersChecked = new();
    public static HashSet<string> PlayersToReport = new();

    public static string PlayerIDs = string.Empty;

    private static bool HasChecked;
    private static string LastRoomChecked = string.Empty;

    private void Start()
    {
        gameObject.AddComponent<PhotonCallbacks>();
        _ = AsyncGetPlayerIDs();

        EasierLog("ARS fully initialized, thank you for helping the gorilla tag modding community!");
    }

    private void Update()
    {
        if (!NetworkSystem.Instance.InRoom)
            return;

        string roomName = PhotonNetwork.CurrentRoom.Name;

        if (!HasChecked || roomName == LastRoomChecked)
            return;

        HasChecked = false;
        PlayersChecked.Clear();
    }

    public static void CheckServer()
    {
        if (PlayersToReport.Count == 0 || !NetworkSystem.Instance.InRoom)
            return;

        foreach (Player player in PhotonNetwork.PlayerListOthers)
            CheckUser(player);

        if (HasChecked)
            return;

        LastRoomChecked = PhotonNetwork.CurrentRoom.Name;
        HasChecked = true;
    }

    public static void CheckUser(Player p)
    {
        if (PlayersChecked.Contains(p))
            return;

        if (!NeedToReport(p))
            return;

        var netPlayer = NetworkSystem.Instance.GetNetPlayerByID(p.ActorNumber);

        foreach (var sbl in GorillaScoreboardTotalUpdater.allScoreboardLines)
        {
            if (sbl.linePlayer != netPlayer)
                continue;

            sbl.reportedToxicity = true;
            sbl.PressButton(true, GorillaPlayerLineButton.ButtonType.Toxicity);
        }

        EasierLog($"Reported user {p.NickName}.");
        PlayersChecked.Add(p);
    }

    private static async Task AsyncGetPlayerIDs()
    {
        PlayerIDs = (await client.GetStringAsync(
            "https://raw.githubusercontent.com/AutoReportSystem/ARSPlayerIDs/refs/heads/main/Player%20Ids.txt")).Trim();

        PlayersToReport = PlayerIDs
            .Split(',')
            .Select(id => id.Trim())
            .Where(id => !string.IsNullOrEmpty(id))
            .ToHashSet();

        EasierLog($"Recieved player ids to report. Count of users: {PlayersToReport.Count()}");
    }

    public static bool NeedToReport(Player p) =>
        PlayersToReport.Contains(p.UserId);

    private static void EasierLog(string msg) =>
        Console.WriteLine($"[ARS LOGGING] {msg}");

    #endregion
}

internal class PhotonCallbacks : MonoBehaviourPunCallbacks
{
    #region Photon Overrides

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        try
        {
            StartCoroutine(DelayedCheckServer());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private IEnumerator DelayedCheckServer()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(2.5f, 10f));
        ARS.CheckServer();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        try
        {
            ARS.CheckUser(newPlayer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    #endregion
}