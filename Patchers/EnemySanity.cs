using Archipelago.MultiClient.Net.Models;
using System.Linq;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class EnemySanity
    {
        // Massive list of patches that handle calling the right send check depending on the enemy type and their state.
        [HarmonyPostfix][HarmonyPatch(typeof(Acrabelle), "State_Death")] static void Acrabelle() => SendEnemyCheck("Acrabelle");
        [HarmonyPostfix][HarmonyPatch(typeof(AstralGolmech), "State_DestroyArms")] static void AstralGolmech_Aaa() => SendEnemyCheck("Astral Golmech (Aaa)");
        [HarmonyPostfix][HarmonyPatch(typeof(AstralGolmech), "State_DestroyRocks")] static void AstralGolmech_Askal() => SendEnemyCheck("Astral Golmech (Askal)");
        [HarmonyPostfix][HarmonyPatch(typeof(AquaTrooper), "State_Death")] static void AquaTrooper(AquaTrooper __instance) => CorruptedEnemy(__instance.faction, "Aqua Trooper");
        [HarmonyPostfix][HarmonyPatch(typeof(Beartle), "State_Death")] static void Beartle(Beartle __instance) => CorruptedEnemy(__instance.faction, "Beartle");
        [HarmonyPostfix][HarmonyPatch(typeof(BeastOne), "State_Death")] static void BeastOne() => SendEnemyCheck("Beast One");
        [HarmonyPostfix][HarmonyPatch(typeof(BeastTwo), "State_Death")] static void BeastTwo() => SendEnemyCheck("Beast Two");
        [HarmonyPostfix][HarmonyPatch(typeof(BeastThree), "State_Death")] static void BeastThree() => SendEnemyCheck("Beast Three");
        [HarmonyPostfix][HarmonyPatch(typeof(BFF2000), "State_Death")] static void BFF2000() => SendEnemyCheck("BFF2000");
        [HarmonyPostfix][HarmonyPatch(typeof(BlastCone), "State_Death")] static void BlastCone() => SendEnemyCheck("Blast Cone");
        [HarmonyPostfix][HarmonyPatch(typeof(Bonecrawler), "State_Death")] static void Bonecrawler() => SendEnemyCheck("Bonecrawler");
        [HarmonyPostfix][HarmonyPatch(typeof(Bonespitter), "State_Death")] static void Bonespitter() => SendEnemyCheck("Bonespitter");
        [HarmonyPostfix][HarmonyPatch(typeof(BoomBeth), "State_Death")] static void BoomBeth() => SendEnemyCheck("Boom Beth");
        [HarmonyPostfix][HarmonyPatch(typeof(Bubblorbiter), "State_Death")] static void Bubblorbiter() => SendEnemyCheck("Bubblorbiter");
        [HarmonyPostfix][HarmonyPatch(typeof(Burro), "State_Death")] static void Burro() => SendEnemyCheck("Burro");
        [HarmonyPostfix][HarmonyPatch(typeof(Cocoon), "State_Death")] static void Cocoon() => SendEnemyCheck("Cocoon");
        [HarmonyPostfix][HarmonyPatch(typeof(BossCorazonLT), "State_KO")] static void CoryBaku2() => SendEnemyCheck("Corazon");
        [HarmonyPostfix][HarmonyPatch(typeof(CowHornSchmup), "State_Death")] static void CowHorn() => SendEnemyCheck("Cow Horn");
        [HarmonyPostfix][HarmonyPatch(typeof(Crabulon), "State_Death")] static void Crabulon() => SendEnemyCheck("Crabulon");
        [HarmonyPostfix][HarmonyPatch(typeof(Crowitzer), "State_Death")] static void Crowitzer() => SendEnemyCheck("Crowitzer");
        [HarmonyPostfix][HarmonyPatch(typeof(Crustaceon), "State_Death")] static void Crustaceon() => SendEnemyCheck("Crustaceon");
        [HarmonyPostfix][HarmonyPatch(typeof(DartHog), "State_Death")] static void DartHog() => SendEnemyCheck("Dart Hog");
        [HarmonyPostfix][HarmonyPatch(typeof(DinoWalker), "State_Death")] static void DinoWalker(DinoWalker __instance) => CorruptedEnemy(__instance.faction, "Dino Walker");
        [HarmonyPostfix][HarmonyPatch(typeof(Discord), "State_Death")] static void Discord() => SendEnemyCheck("Discord");
        [HarmonyPostfix][HarmonyPatch(typeof(DiscordDoor), "State_Death")] static void DiscordMK2() => SendEnemyCheck("Discord");
        [HarmonyPostfix][HarmonyPatch(typeof(DrakeCoccoon), "State_Death")] static void DrakeCocoon() => SendEnemyCheck("Drake Cocoon");
        [HarmonyPostfix][HarmonyPatch(typeof(DrakeFly), "State_Death")] static void DrakeFly(DrakeFly __instance) => CorruptedEnemy(__instance.faction, "Drake Fly");
        [HarmonyPostfix][HarmonyPatch(typeof(DropletShip), "State_Death")] static void DropletShip() => SendEnemyCheck("Droplet Ship");
        [HarmonyPostfix][HarmonyPatch(typeof(Duality), "State_Death")] static void Duality() => SendEnemyCheck("Duality");
        [HarmonyPostfix][HarmonyPatch(typeof(Durugin), "State_Death")] static void Durugin() => SendEnemyCheck("Durugin");
        [HarmonyPostfix][HarmonyPatch(typeof(FireHopper), "State_Death")] static void FireHopper() => SendEnemyCheck("Fire Hopper");
        [HarmonyPostfix][HarmonyPatch(typeof(Flamingo), "State_Death")] static void Flamingo(Flamingo __instance) => CorruptedEnemy(__instance.faction, "Flamingo");
        [HarmonyPostfix][HarmonyPatch(typeof(FlashMouth), "State_Death")] static void FlashMouth(FlashMouth __instance) => CorruptedEnemy(__instance.faction, "Flash Mouth");
        [HarmonyPostfix][HarmonyPatch(typeof(FlyingSaucer), "State_Death")] static void FlyingSaucer() => SendEnemyCheck("Flying Saucer");
        [HarmonyPostfix][HarmonyPatch(typeof(FoldingSnake), "State_Death")] static void FoldingSnake() => SendEnemyCheck("Folding Snake");
        [HarmonyPostfix][HarmonyPatch(typeof(GatHog), "State_Death")] static void GatHog() => SendEnemyCheck("Gat Hog");
        [HarmonyPostfix][HarmonyPatch(typeof(Girder), "State_Death")] static void Girder() => SendEnemyCheck("Girder");
        [HarmonyPostfix][HarmonyPatch(typeof(GnawsaLock), "State_Death")] static void GnawsaLock() => SendEnemyCheck("Gnawsa Lock");
        [HarmonyPostfix][HarmonyPatch(typeof(Hellpo), "State_Death")] static void Hellpo() => SendEnemyCheck("Hellpo");
        [HarmonyPostfix][HarmonyPatch(typeof(Herald), "State_Death")] static void Herald() => SendEnemyCheck("Herald");
        [HarmonyPostfix][HarmonyPatch(typeof(HijackedPoliceCar), "State_Dead")] static void HijackedPoliceCar() => SendEnemyCheck("Hijacked Police Car");
        [HarmonyPostfix][HarmonyPatch(typeof(HotPlate), "State_Death")] static void HotPlate() => SendEnemyCheck("Hot Plate");
        [HarmonyPostfix][HarmonyPatch(typeof(HundredDrillion), "State_Death")] static void HundredDrillion() => SendEnemyCheck("Hundred Drillion");
        [HarmonyPostfix][HarmonyPatch(typeof(Iris), "State_Death")] static void Iris() => SendEnemyCheck("Iris");
        [HarmonyPostfix][HarmonyPatch(typeof(Jawdrop), "State_Death")] static void Jawdrop() => SendEnemyCheck("Jawdrop");
        [HarmonyPostfix][HarmonyPatch(typeof(Kakugan), "State_Death")] static void Kakugan() => SendEnemyCheck("Kakugan");
        [HarmonyPostfix][HarmonyPatch(typeof(Keon), "State_Death")] static void Keon() => SendEnemyCheck("Keon");
        [HarmonyPostfix][HarmonyPatch(typeof(KoiCannon), "State_Death")] static void KoiCannon() => SendEnemyCheck("Koi Cannon");
        [HarmonyPostfix][HarmonyPatch(typeof(LemonBread), "State_Death")] static void LemonBread() => SendEnemyCheck("Lemon Bread");
        [HarmonyPostfix][HarmonyPatch(typeof(LineCutter), "State_Death")] static void LineCutter() => SendEnemyCheck("Line Cutter");
        [HarmonyPostfix][HarmonyPatch(typeof(Macer), "State_Death")] static void Macer() => SendEnemyCheck("Macer");
        [HarmonyPostfix][HarmonyPatch(typeof(Manpowa), "State_Death")] static void Manpowa() => SendEnemyCheck("Manpowa");
        [HarmonyPostfix][HarmonyPatch(typeof(Mantis), "State_Death")] static void Mantis() => SendEnemyCheck("Mantis");
        [HarmonyPostfix][HarmonyPatch(typeof(MergaBlueMoon), "State_Death")] static void MergaBlueMoon() => SendEnemyCheck("Merga (Blue Moon)");
        [HarmonyPostfix][HarmonyPatch(typeof(MergaBloodMoon), "State_Death")] static void MergaBloodMoon() => SendEnemyCheck("Merga (Blood Moon)");
        [HarmonyPostfix][HarmonyPatch(typeof(MergaSupermoon), "State_Death")] static void MergaSuperMoon() => SendEnemyCheck("Merga (Super Moon)");
        [HarmonyPostfix][HarmonyPatch(typeof(MergaEclipse), "State_Death")] static void MergaEclipse() => SendEnemyCheck("Merga (Eclipse)");
        [HarmonyPostfix][HarmonyPatch(typeof(MergaLilith), "State_Death")] static void MergaLilith() => SendEnemyCheck("Merga (Lilith)");
        [HarmonyPostfix][HarmonyPatch(typeof(PlayerBossMerga), "State_KO2")] static void Merga() => SendEnemyCheck("Merga");
        [HarmonyPostfix][HarmonyPatch(typeof(MeteorRoller), "State_Death")] static void MeteorRoller() => SendEnemyCheck("Meteor Roller");
        [HarmonyPostfix][HarmonyPatch(typeof(MonsterCube), "State_Death")] static void MonsterCube() => SendEnemyCheck("Monster Cube");
        [HarmonyPostfix][HarmonyPatch(typeof(Peller), "State_Death")] static void Peller() => SendEnemyCheck("Peller");
        [HarmonyPostfix][HarmonyPatch(typeof(Pendurum), "State_Death")] static void Pendurum(Pendurum __instance) => CorruptedEnemy(__instance.faction, "Pendurum");
        [HarmonyPostfix][HarmonyPatch(typeof(PogoSnail), "State_Death")] static void PogoSnail() => SendEnemyCheck("Pogo Snail");
        [HarmonyPostfix][HarmonyPatch(typeof(Prawn), "State_Death")] static void Prawn() => SendEnemyCheck("Prawn");
        [HarmonyPostfix][HarmonyPatch(typeof(PrawnToBeWild), "State_Death")] static void PrawnToBeWild() => SendEnemyCheck("Prawn To Be Wild");
        [HarmonyPostfix][HarmonyPatch(typeof(ProtoPincer), "State_Death")] static void ProtoPincer() => SendEnemyCheck("Proto Pincer");
        [HarmonyPostfix][HarmonyPatch(typeof(RailDriver), "State_Death")] static void RailDriver() => SendEnemyCheck("Rail Driver");
        [HarmonyPostfix][HarmonyPatch(typeof(Raytracker), "State_Death")] static void Raytracker(Raytracker __instance) => CorruptedEnemy(__instance.faction, "Raytracker");
        [HarmonyPostfix][HarmonyPatch(typeof(RifleTrooper), "State_Death")] static void RifleTrooper() => SendEnemyCheck("Rifle Trooper");
        [HarmonyPostfix][HarmonyPatch(typeof(Rosebud), "State_Death")] static void Rosebud() => SendEnemyCheck("Rosebud");
        [HarmonyPostfix][HarmonyPatch(typeof(SawShrimp), "State_Death")] static void SawShrimp() => SendEnemyCheck("Saw Shrimp");
        [HarmonyPostfix][HarmonyPatch(typeof(Sentinel), "State_Death")] static void Sentinel() => SendEnemyCheck("Sentinel");
        [HarmonyPostfix][HarmonyPatch(typeof(ShellGrowth), "State_Death")] static void ShellGrowth() => SendEnemyCheck("Shell Growth");
        [HarmonyPostfix][HarmonyPatch(typeof(Shellvitate), "State_Death")] static void Shellvitate() => SendEnemyCheck("Shell Growth");
        [HarmonyPostfix][HarmonyPatch(typeof(Shockula), "State_Death")] static void Shockula() => SendEnemyCheck("Shockula");
        [HarmonyPostfix][HarmonyPatch(typeof(Softballer), "State_Death")] static void Softballer() => SendEnemyCheck("Softballer");
        [HarmonyPostfix][HarmonyPatch(typeof(SpyTurretus), "State_Death")] static void SpyTurretus(SpyTurretus __instance) => CorruptedEnemy(__instance.faction, "Spy Turretus");
        [HarmonyPostfix][HarmonyPatch(typeof(Stahp), "State_Death")] static void Stahp() => SendEnemyCheck("Stahp");
        [HarmonyPostfix][HarmonyPatch(typeof(StormSlider), "State_Death")] static void StormSlider() => SendEnemyCheck("Storm Slider");
        [HarmonyPostfix][HarmonyPatch(typeof(SwordTrooper), "State_Death")] static void SwordTrooper() => SendEnemyCheck("Sword Trooper");
        [HarmonyPostfix][HarmonyPatch(typeof(SwordWingSchmup), "State_Death")] static void SwordWing() => SendEnemyCheck("Sword Wing");
        [HarmonyPostfix][HarmonyPatch(typeof(SyntaxSpider), "State_Death")] static void SyntaxSpider() => SendEnemyCheck("Syntax Spider");
        [HarmonyPostfix][HarmonyPatch(typeof(TitanArmor), "State_Death")] static void TitanArmor() => SendEnemyCheck("Titan Armor");
        [HarmonyPostfix][HarmonyPatch(typeof(TombstoneTurretus), "State_Death")] static void TombstoneTurretus() => SendEnemyCheck("Tombstone Turretus");
        [HarmonyPostfix][HarmonyPatch(typeof(Torcher), "State_Death")] static void Torcher() => SendEnemyCheck("Torcher");
        [HarmonyPostfix][HarmonyPatch(typeof(TowerCannon), "State_Death")] static void TowerCannon() => SendEnemyCheck("Tower Cannon");
        [HarmonyPostfix][HarmonyPatch(typeof(ToyDecoy), "State_Death")] static void ToyDecoy() => SendEnemyCheck("Toy Decoy");
        [HarmonyPostfix][HarmonyPatch(typeof(Traumagotcha), "State_Death")] static void Traumagotcha(Traumagotcha __instance) => CorruptedEnemy(__instance.faction, "Traumagotcha");
        [HarmonyPostfix][HarmonyPatch(typeof(TriggerJoy), "State_Death")] static void TriggerJoy() => SendEnemyCheck("Trigger Joy");
        [HarmonyPostfix][HarmonyPatch(typeof(Lancer), "State_Death")] static void TriggerLancer() => SendEnemyCheck("Trigger Lancer");
        [HarmonyPostfix][HarmonyPatch(typeof(Troopish), "State_Death")] static void Troopish() => SendEnemyCheck("Troopish");
        [HarmonyPostfix][HarmonyPatch(typeof(TunnelDriver), "State_Death")] static void TunnelDriver() => SendEnemyCheck("Tunnel Driver");
        [HarmonyPostfix][HarmonyPatch(typeof(Turretus), "State_Death")] static void Turretus(Turretus __instance) => CorruptedEnemy(__instance.faction, "Turretus");
        [HarmonyPostfix][HarmonyPatch(typeof(WaterHopper), "State_Death")] static void WaterHopper() => SendEnemyCheck("Water Hopper");
        [HarmonyPostfix][HarmonyPatch(typeof(WeatherFace), "State_Death")] static void WeatherFace() => SendEnemyCheck("Weather Face");
        [HarmonyPostfix][HarmonyPatch(typeof(WolfArmor), "State_Death")] static void WolfArmor() => SendEnemyCheck("Wolf Armour");
        [HarmonyPostfix][HarmonyPatch(typeof(WoodHopper), "State_Death")] static void WoodHopper() => SendEnemyCheck("Wood Hopper");
        [HarmonyPostfix][HarmonyPatch(typeof(ZombieTrooper), "State_Death")] static void ZombieTrooper() => SendEnemyCheck("Zombie Trooper");

        /// <summary>
        /// Handles sending out checks for the Code Black afflicted enemies (as they share the same script as their normal versions).
        /// TODO: If there's one that doesn't have their faction set to Alien then this will fall apart.
        /// </summary>
        static void CorruptedEnemy(string faction, string enemyName)
        {
            if (faction == "Alien") SendEnemyCheck($"Corrupted {enemyName}");
            else SendEnemyCheck(enemyName);
        }
        
        /// <summary>
        /// Handles sending out both Astral Golmech checks upon defeat, as the Destroy[x] function only runs for the first pilot defeated.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AstralGolmech), "State_Death")]
        static void AstralGolmech()
        {
            SendEnemyCheck("Astral Golmech (Aaa)");
            SendEnemyCheck("Astral Golmech (Askal)");
        }

        /// <summary>
        /// Handles sending out the checks for the bosses that are set up like player characters.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerBoss), "State_KO")] 
        static void GenericPlayerBoss(PlayerBoss __instance)
        {
            switch (__instance.GetType().Name)
            {
                case "PlayerBossAskal": SendEnemyCheck("Askal"); return;
                case "PlayerBossCarol": SendEnemyCheck("Carol"); return;
                case "PlayerBossCorazon": SendEnemyCheck("Corazon"); return;
                case "PlayerBossGong": SendEnemyCheck("General Gong"); return;
                case "PlayerBossKalaw": SendEnemyCheck("Captain Kalaw"); return;
                case "PlayerBossLilac": SendEnemyCheck("Lilac"); return;
                case "PlayerBossMerga": SendEnemyCheck("Merga"); return;
                case "PlayerBossMilla": SendEnemyCheck("Milla"); return;
                case "PlayerBossNeera": SendEnemyCheck("Neera"); return;
                case "PlayerBossSerpentine": SendEnemyCheck("Serpentine"); return;

                // If the plugin is compiled in Debug Mode, then log the KO'd boss.
                default:
                    #if DEBUG
                    Plugin.consoleLog.LogInfo(__instance.GetType().Name);
                    #endif
                    return;
            }
        }

        /// <summary>
        /// Sends the provided location check, if it exists.
        /// </summary>
        static void SendEnemyCheck(string enemyName)
        {
            // Check if we've already handled this enemy's location.
            if (Plugin.save.EnemySanityIDs.ContainsKey(enemyName))
                return;

            // Get the location for this boss type.
            long locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", enemyName);

            // Complete this location check if it exists.
            if (Helpers.CheckLocationExists(locationIndex) && !Plugin.session.Locations.AllLocationsChecked.Contains(locationIndex))
            {
                Plugin.session.Locations.CompleteLocationChecks(locationIndex);

                // Scout the location we just completed.
                ScoutedItemInfo _scoutedLocationInfo = null;
                Plugin.session.Locations.ScoutLocationsAsync(HandleScoutInfo, [locationIndex]);

                // Pause operation until the location is scouted.
                while (_scoutedLocationInfo == null)
                    System.Threading.Thread.Sleep(1);

                // Add a message to the queue if this item is for someone else.
                if (_scoutedLocationInfo.Player.Name != Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot))
                    Plugin.sentMessageQueue.Add($"Found {_scoutedLocationInfo.Player.Name}'s {_scoutedLocationInfo.ItemName}.");

                // Save this location so we don't check it multiple times.
                Plugin.save.EnemySanityIDs.Add(enemyName, locationIndex);

                void HandleScoutInfo(Dictionary<long, ScoutedItemInfo> scoutedLocationInfo) => _scoutedLocationInfo = scoutedLocationInfo.First().Value;
            }
        }
    }
}
