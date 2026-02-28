// VRC RMS 1.0.0
// Uzer Tekton
// MIT License


using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Network;
using VRC.SDK3.Rendering;
using VRC.SDKBase;
using VRC.Udon.Common.Enums;
using Random = UnityEngine.Random;


namespace UzerTekton.VRCRMS
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VRCReactiveModerationSystem : UdonSharpBehaviour
    {
        [SerializeField] private string[] moderators;

        [SerializeField] private string[] banList;

        private bool _isLocalModerationActionApplied;

        [SerializeField] private GameObject noticeGameObject;

        [SerializeField] private GameObject jailGameObject;

        private VRCPlayerApi[] _players;

        [SerializeField] private bool isDebugLogging = true;


        private void Start()
        {
            Log("VRC Reactive Moderation System starting (Version 1.0.0)");


            // Checking if the lists are setup correctly
            if (moderators == null)
            {
                Log($"<color=red>Moderator</color> array is not filled in, please fix in editor");
                moderators = new string[1];
            }

            if (banList == null)
            {
                Log($"<color=red>Ban List</color> array is not filled in, please fix in editor");
                banList = new string[1];
            }

            for (int i = 0; i < moderators.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(moderators[i]))
                {
                    Log($"Moderator array index [<color=red>{i}</color>] is empty or null, please fix in editor");
                    moderators[i] = "";
                }
            }

            for (int i = 0; i < banList.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(banList[i]))
                {
                    Log($"Ban List array index [<color=red>{i}</color>] is empty or null, please fix in editor");
                    banList[i] = "";
                }
            }


            // Checking notice object and filling in the notice text
            if (!noticeGameObject)
            {
                Log($"<color=red>Notice GameObject</red> is unreferenced, please fix in editor");
            }
            else
            {
                if (noticeGameObject.GetComponent<TMP_Text>())
                {
                    string moderatorsString = "";
                    for (int i = 0; i < moderators.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(moderators[i]))
                        {
                            moderatorsString += $"{moderators[i]}"; // Adding name
                            moderatorsString += ", "; // Adding a comma and space
                        }
                    }

                    moderatorsString = moderatorsString.TrimEnd(',', ' ');

                    string banListString = "";
                    for (int i = 0; i < banList.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(banList[i]))
                        {
                            banListString += $"{banList[i]}"; // Adding name
                            banListString += ", "; // Adding a comma and space
                        }
                    }

                    banListString = banListString.TrimEnd(',', ' ');

                    noticeGameObject.GetComponent<TMP_Text>().text = noticeGameObject.GetComponent<TMP_Text>().text.Replace("<Moderators autofill>", moderatorsString);
                    noticeGameObject.GetComponent<TMP_Text>().text = noticeGameObject.GetComponent<TMP_Text>().text.Replace("<Ban List autofill>", banListString);
                }
            }


            // Checking jail decorations
            if (!jailGameObject)
            {
                Log($"<color=red>Jail GameObject</red> is unreferenced, please fix in editor");
            }
            else
            {
                jailGameObject.SetActive(false);
            }


            Log("Start complete");
        }

        // The main timing for checks
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            CheckAllPlayers();
        }

        // If the local player is already banned, respawn will just trigger the check again
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player.isLocal) CheckAllPlayers();
        }

        // Entry point for checks
        // Check all players
        private void CheckAllPlayers()
        {
            _players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(_players);
           
            if (!CheckIsRMSEnabled()) return;

            for (int i = 0; i < _players.Length; i++)
            {
                if (CheckIsOnBanList(_players[i])) _ModerationAction(_players[i]);
            }
        }


        // If instance owner or any moderator is in the instance, RMS is enabled
        private bool CheckIsRMSEnabled()
        {
            // Check if the instance owner is in the instance
            for (int i = 0; i < _players.Length; i++)
            {
                if (_players[i].isInstanceOwner)
                {
                    Log($"Instance owner {_players[i].displayName} is in instance");
                    return true;
                }
            }

            // Check if any moderator is in the instance
            for (int i = 0; i < moderators.Length; i++)
            {
                for (int j = 0; j < _players.Length; j++)
                {
                    if (_players[j].displayName == moderators[i])
                    {
                        Log($"Moderator {moderators[i]} is in instance");
                        return true;
                    }
                }
            }

            return false;
        }

        // Check if the target player is on the ban list
        private bool CheckIsOnBanList(VRCPlayerApi player)
        {
            if (player.isInstanceOwner)
            {
                Log($"Ban List player <color=red>{player.displayName}</color> detected, but is instance owner");
                return false; // Instance owner gains immunity
            }

            for (int i = 0; i < banList.Length; i++)
            {
                if (player.displayName == banList[i])
                {
                    Log($"Ban List player <color=red>{banList[i]}</color> detected");
                    return true;
                }
            }

            return false;
        }


        // Apply moderation action to a target player
        private void _ModerationAction(VRCPlayerApi player)
        {
            // If target player is local, apply locally
            if (player.isLocal)
            {
                Log($"Local player <color=red>{player.displayName}</color> is moderated");
                _ModerationActionLocal();
            }

            // If target player is remote, apply to remote player
            else
            {
                Log($"Remote player <color=red>{player.displayName}</color> is moderated");
                _ModerationActionRemote(player);
            }
        }


        // ***Edit this method to customize your moderation effects on a banned remote player***
        // Apply moderation actions to a remote player
        private void _ModerationActionRemote(VRCPlayerApi player)
        {
            // Muted
            player.SetVoiceGain(0);
            player.SetVoiceDistanceNear(0);
            player.SetVoiceDistanceFar(0);
            player.SetVoiceVolumetricRadius(0);
            player.SetAvatarAudioGain(0);
            player.SetAvatarAudioNearRadius(0);
            player.SetAvatarAudioFarRadius(0);
            player.SetAvatarAudioVolumetricRadius(0);


            // Add your own effects here that should apply to a banned remote player
            // *
            // E.g. Remove from game mechanics, etc.
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 


            // End of moderation effects on remote player
        }


        // ***Edit this method to customize your moderation effects on the local player if banned***
        // Apply moderation actions to local player
        public void _ModerationActionLocal()
        {
            // Within this method, we can no longer trust the outer variables because local player may be hacking and editing the values.


            isDebugLogging = false;
            _isLocalModerationActionApplied = true; // Dummy variable for hack detection and decoration


            // Drop any held pickup
            if (Utilities.IsValid(Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Left))) Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Left).Drop();
            if (Utilities.IsValid(Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Right))) Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Right).Drop();

            // You might want to add your own code here to respawn all the other pickups in case the hacker has any ownership


            // Teleport to jail
            // Default position is further than 2048 from origin, if your map is near origin and your camera has a default far clip of 1000 then the jail should normally be well beyond other player's rendering distance
            Vector3 jailPosition = new Vector3(Random.Range(-4096f, -2048f), -50, Random.Range(-4096f, -2048f));
            Networking.LocalPlayer.TeleportTo(jailPosition, Quaternion.identity);


            // Teleport the drone as safety measure
            VRCDroneApi localDrone = Networking.LocalPlayer.GetDrone();
            if (Utilities.IsValid(localDrone) && localDrone.IsDeployed())
            {
                localDrone.TeleportTo(jailPosition, Quaternion.identity);
                localDrone.SetVelocity(Vector3.zero);
            }


            // Move the jail object to jail position
            if (jailGameObject)
            {
                jailGameObject.SetActive(true);
                jailGameObject.transform.SetParent(null); // Putting this at root to decouple any possible transform problem
                jailGameObject.transform.position = jailPosition;
                jailGameObject.transform.rotation = Quaternion.identity;
                jailGameObject.layer = 8; // No stickers allowed
            }

            // Move the jail notice to the jail position for ToS compliance
            if (noticeGameObject)
            {
                noticeGameObject.transform.SetParent(null); // Putting this at root to decouple any possible transform problem
                noticeGameObject.transform.position = jailPosition + Vector3.forward * 0.5f + Vector3.up;
                noticeGameObject.transform.rotation = Quaternion.identity;
                noticeGameObject.layer = 8; // No stickers allowed
            }


            // Deafen
            _players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(_players);

            for (int i = 0; i < _players.Length; i++)
            {
                _players[i].SetVoiceGain(0);
                _players[i].SetVoiceDistanceNear(0);
                _players[i].SetVoiceDistanceFar(0);
                _players[i].SetVoiceVolumetricRadius(0);
                _players[i].SetAvatarAudioGain(0);
                _players[i].SetAvatarAudioNearRadius(0);
                _players[i].SetAvatarAudioFarRadius(0);
                _players[i].SetAvatarAudioVolumetricRadius(0);
            }


            // Immobilize
            Networking.LocalPlayer.SetWalkSpeed(0);
            Networking.LocalPlayer.SetRunSpeed(0);
            Networking.LocalPlayer.SetStrafeSpeed(0);
            Networking.LocalPlayer.SetJumpImpulse(0);

            // Networking.LocalPlayer.SetGravityStrength(0); // The jail floor needs gravity to stand on to look normal for stealth reasons
            Networking.LocalPlayer.Immobilize(true);
            Networking.LocalPlayer.SetVelocity(Vector3.zero);


            // Limit view range and rendering layers
            VRCCameraSettings.ScreenCamera.NearClipPlane = 0.05f;
            VRCCameraSettings.ScreenCamera.FarClipPlane = 50f;
            VRCCameraSettings.ScreenCamera.CullingMask = 1 << 8; // Only render the layer of the notice for ToS compliance
            VRCCameraSettings.ScreenCamera.ClearFlags = CameraClearFlags.SolidColor;
            VRCCameraSettings.ScreenCamera.BackgroundColor = Color.white;
            VRCCameraSettings.ScreenCamera.LayerCullDistances = null;


            // Repeat for photo camera
            VRCCameraSettings.PhotoCamera.NearClipPlane = 0.05f;
            VRCCameraSettings.PhotoCamera.FarClipPlane = 50f;

            // VRCCameraSettings.PhotoCamera.CullingMask = 1 << 8; // Only works for ScreenCamera, do not uncomment
            VRCCameraSettings.PhotoCamera.ClearFlags = CameraClearFlags.SolidColor;
            VRCCameraSettings.PhotoCamera.BackgroundColor = Color.white;
            VRCCameraSettings.ScreenCamera.LayerCullDistances = null;


            // Add your own effects here that should apply to a banned local player
            // *
            // E.g. Reset persistence data, deactivate all the GameObjects etc.
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 


            // End of moderation effects on local player


            // Anti-cheat area, do not modify
            // Repeat this method periodically for anti-cheat
            SendCustomEventDelayedSeconds("_ModerationActionLocal", (Random.Range(1f, 7f) + Random.Range(1f, 7f) + Random.Range(1f, 7f)) * Random.Range(1, 5));

            // Set up Anti-hack
            string memoryCheckingA = MemoryCheck();
            Log(memoryCheckingA);

            // For hack detection
            bool isNameStillOnBanList = false;
            for (int i = 0; i < banList.Length; i++)
            {
                if (Networking.LocalPlayer.displayName == banList[i]) isNameStillOnBanList = true;
            }

            // If hack is detected, try to fix the variables every few physics frame, for aesthetic reasons
            if (!_isLocalModerationActionApplied || isDebugLogging || !isNameStillOnBanList)
            {
                SendCustomEventDelayedFrames("_ModerationActionLocal", (Time.deltaTime < 1f / 24f) ? Random.Range(0, 256) : Random.Range(0, 65536), EventTiming.FixedUpdate);

                string memoryCheckingB = MemoryCheck();
                if (memoryCheckingA != memoryCheckingB) Log($"{memoryCheckingB} {MemoryCheck()}");

                banList = new string[256];
                for (int i = 0; i < banList.Length; i++)
                {
                    banList[i] = Networking.LocalPlayer.displayName;
                }
            }
        }


        private void Log(string message)
        {
            if (!isDebugLogging) return;
            Debug.Log($"[VRC RMS] <color=grey>Frame: {Time.frameCount} TimeInRoom: {Stats.TimeInRoom}</color> {message}");
        }


        // Anti-hack, do not modify
        private static string MemoryCheck()
        {
            string[] ops = { "PUSH", "LOAD", "STORE", "XOR", "AES_ENC", "SHA256", "BASE64", "JIF", "CALL" };
            string[] syms = { "_heap", "_nonce", "_iv", "_salt", "_buf", "_hash", "_arg0" };
            const string b64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

            string op = ops[Random.Range(0, ops.Length)];

            string hex = "0x";
            for (int i = 0; i < 6; i++) hex += Random.Range(0, 16).ToString("X");

            string hexLong = "0x";
            for (int i = 0; i < 12; i++) hexLong += Random.Range(0, 16).ToString("X");

            string base64 = "";
            for (int i = 0; i < 20; i++) base64 += b64[Random.Range(0, b64.Length)];
            base64 += "==";

            if (op == "PUSH") return "PUSH " + (Random.value > 0.5f ? syms[Random.Range(0, syms.Length)] : hex);
            if (op == "LOAD") return "LOAD " + syms[Random.Range(0, syms.Length)];
            if (op == "STORE") return "STORE " + syms[Random.Range(0, syms.Length)];
            if (op == "XOR") return "XOR " + hex;
            if (op == "AES_ENC") return "AES_ENC " + hexLong + " IV=" + hex + " SALT=" + hex + " MODE=CBC PAD=PKCS7";
            if (op == "SHA256") return "SHA256 state=" + hex + " block=" + hexLong + " round=" + Random.Range(0, 64);
            if (op == "BASE64") return "BASE64 \"" + base64 + "\" -> " + syms[Random.Range(0, syms.Length)];
            if (op == "JIF") return "JUMP_IF_FALSE L_" + hex + " VERIFY_SIG";
            if (op == "CALL") return "CALL \"System.Security.Crypto::__Hash_" + hex + "\"";

            return "NOP";
        }
    }
}
