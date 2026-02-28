# VRC Reactive Moderation System
A high performance ToS compliant Udon based moderation system

MIT License

> [!CAUTION]
> Read the latest official Creator Guidelines and ToS before using this system.

## The problem
The VRChat [Creator Guidelines](https://hello.vrchat.com/creator-guidelines) (enforced by the ToS) disallow a "block list" based preemptive moderation system being used in worlds. World authors are being punished for blocking known-hackers preemptively.

## The solution
Creator Guidelines specifically allow:
- A Udon based moderation system, as long as:
  - Your rules are displayed at the entrance.
  - You inform the moderated player what is the moderation action applied, and why.
- An "allow list" to enable certain features. (Which can include the Udon based moderation system.)

To meet *exactly* these criteria, the **VRC Reactive Moderation System (VRC RMS)** gives "allow listed" individuals ("Moderators") the feature of automatically apply moderation action to players based on rules everyone agreed on. All players must agree to use a criteria of "appearing on the ban list" being the "rules of the game" and the reason of moderation actions. The ban list can be an arbitrary list prepared by the world author because it becomes part of an agreed set of rules.

Moderation actions can be anything the world creator wants. The Creator Guidelines suggests teleporting to a separate area, or muted, or being disabled from interacting with the world. But you are free to invent a different set of actions. The prefab includes teleporting and muted and immobilized by default.

> [!IMPORTANT]
> It is important to note that this system is not a preemptive "block list", but a Udon based moderation system.
>
> All this system does is actioning a Moderator's reaction to another individual. This system does not perform any preemptive moderation i.e. apply any cross-instance blanket ban without the use of Moderators and without an agreed set of rules. The bans expire automatically when one leaves the instance. This is the key design to achieving a ToS compliant system.

> [!NOTE]
> To comply with ToS, you must display your rules at the world entrance. Use this example or a variation of it:
> (No need to edit the names in the text, the prefab autofills the lists for you)
> ```
> This world is protected by VRC Reactive Moderation System ("VRC RMS"), a ToS compliant Udon based moderation system. By continuing to stay in this instance, you agree to the following rules:
> 1. Moderators appointed by the World Author may apply Moderation Actions to any player at their discretion.
> 2. Moderators agree to apply Moderation Actions based on a Vibe Check.
> 3. Moderators agree to conduct a Vibe Check on all players based on whether a player appears on the Ban List prescribed by the World Author.
> 4. Moderators agree to conduct the Vibe Check with the assistance of the VRC RMS.
> 5. Moderators agree to apply Moderation Actions with the assistance of the VRC RMS.
> 6. Moderation Actions include being teleported to a separate area, muted, and immobilized.
> 7. If Moderation Actions have been applied to you, the reason would be you have failed a Vibe Check by a Moderator. You are free to leave this instance, and enter a private instance to continue your activity.
> 8. The instance owner is automatically granted Moderator status temporarily.
> 9. The instance owner is automatically excluded from the Ban List temporarily for ToS compliance reasons.
> 10. Moderators: Moderator A, Moderator B, You Own Name, <Insert names>
> 11. Ban List: Hacker A, Hacker B, <Insert names>
> ```

> [!NOTE]
> To comply with ToS, you must tell the moderated player what moderation actions have been applied and why. The prefab automatically moves the notice to the teleported "jail" location so the player can continue reading it.

## Features

- Very optimized architecture with minimal CPU and memory usage
- Very easy to install and customize
- Extremely effective at moderation based on agreed predefined rules
- Very safe, memory and network hardened against hacking attempts
- World author has full control of ban list, there is zero chance of abuse by any Moderator or instance owner


## Example situations

| Situation | Result | Reason |
| --- | --- | --- |
| A Moderator joins the instance. | **All hackers are instabanned** | Banned by the Moderator. |
| Instance owner stays in the instance. | **All hackers are instabanned** | Instance owner gains moderator status automatically. |
| Instance owner has left the instance. There are no Moderators. A hacker enters the world. | **No action** | There is no Moderator available to ban the hacker. Tor ToS reasons, preemptive bans are not actioned. |
| Hacker is the instance owner. There are no Moderators. | **No action** | Instance owner gains moderator status (which *could* ban themselves *in theory*), but instance owner is also automatically excluded from ban list. |
| Hacker is the instance owner. A Moderator joins the instance. | **No action** | For ToS reasons, instance owner is automatically excluded from ban list. |
| Hacker A is the instance owner. There are no Moderators. Hacker B enters the world. | **Hacker A is not banned, but Hacker B is instabanned** | Instance owner gains moderator status (thus banning Hacker B) and is automatically excluded from ban list. |
| A hacker was banned, but the instance owner and all Moderators leave the instance. | **The hacker remains banned** | There is no unban functionality. Leave the instance to unban yourself.|
| A hacker was banned, but the instance owner and all Moderators leave the instance. The hacker rejoins the instance. | **No action** | There is no Moderator available to ban the hacker. Bans expire once the hacker leaves the instance.|


## How to use
Requires SDK `3.10.2` or above and Unity `2022.3.22f1`.

1. Download the prefab from the releases page. Drop it into your Assets window into any folder such as `Assets/Prefabs/VRCRMS/`.

2. Place an instance of the prefab `VRCRMS` into your scene. You can place it anywhere, it literally does not matter because it is invisible, but it may be easier for you to find at `(0,0,0)`.

3. Add names to the `moderators` array from the Inspector window. If any Moderator is in the instance, RMS will be enabled.

4. Add names to the `banList` array from the Inspector window. If any banned name is in the instance, and if RMS is enabled, they will be moderated.

>[!NOTE]
> There is no need to manually add names to the notice text.
> The prefab will automatically fill in the `TMP_Text` by replacing `"<Moderators autofill>"` and `"<Ban List autofill>"` with the actual lists at `Start()`.

5. Position the `VRCRMSNotice` near your world entrance so that anyone entering can read them.

Done.

The following are optional. Please read carefully if you want to customize the prefab.

6. You can edit the notice text on the `VRCRMSNotice` GameObject, so that the rules are suitable for you.
Reminder: there is no need to edit the name lists here because the script automatically fills them in at `Start()`.

7. If you are making the prefab from scratch using the source code, you will need to create two GameObjects for referencing as below:
  - Ensure the `VRCRMSNotice` GameObject is referenced in `VRCRMS`. This is the notice board for displaying the rules.
    - By default, `VRCRMSNotice` is a child GameObject of `VRCRMS`.
    - In the event of teleport to jail, `VRCRMSNotice` will unparent itself and get teleported along with the jailed player so they can still read it for ToS compliance reasons.
  - Ensure the `VRCRMSJail` GameObject is referenced in `VRCRMS`. This is a parent object for helping you customizing your jail.
    - By default, `VRCRMSJail` is a child GameObject of `VRCRMS`.
    - In the event of teleport to jail, `VRCRMSJail` will unparent itself and get teleported to the jailed player position (the root position at the feet).
    - `VRCRMSJail` is an empty GameObject that represents the root position of the jailed player. Use this as the parent GameObject for customizing the jail, for example, the prefab has an (invisible) cube below the root position for the jailed player to stand on. (Without a floor, the avatar may freak out in the free fall animation thus alerting the jailed player.)
    
8. Edit the code to achieve your own desired moderation effects.
  - `_ModerationActionRemote()` contains the effects that apply on a banned **remote** player. Use this for setting voice gain, etc.
  - `_ModerationActionLocal()` contains the effects that apply on a banned **local** player. Use this for teleport, etc.

> [!NOTE]
> If you have multiple Udon based moderation system in your world, please make sure they do not interfere each other.

## Version history

#### 1.0.0
2026-02-28  
Release

## Contact
Discord: https://discord.gg/yG4HnBM8Du
