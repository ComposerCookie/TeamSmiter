using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSmiter
{
    public class DataPipeline
    {
        List<Map> maps;
        List<Turret> turrets;
        List<God> gods;
        List<Graphics> miscanellous;

        public DataPipeline()
        {
            maps = new List<Map>();
            turrets = new List<Turret>();
            gods = new List<God>();
            miscanellous = new List<Graphics>();

            GenerateFirstTime();
        }

        public List<Map> MapsList { get { return maps; } }
        public List<Turret> TurretsList { get { return turrets; } }
        public List<God> GodList { get { return gods; } }
        public List<Graphics> Miscanellous { get { return miscanellous; } }
        public string GraphicPath { get; set; }
        public string MapPath { get; set; }
        public string TurretPath { get; set; }
        public string GodPath { get; set; }
        public string MiscanellousPath { get; set; }
        public int GodFaceSize { get; set; }
        public int GodMiniFaceSize { get; set; }

        public void GenerateFirstTime()
        {
            GraphicPath = "Graphics";
            MapPath = "Maps";
            GodPath = "Gods";
            TurretPath = "Turrets";
            MiscanellousPath = "Miscanellous";

            GodFaceSize = 96;
            GodMiniFaceSize = 48;

            maps.Add(new Map(GraphicPath + "/" + MapPath + "/" + "ConquestMap.png", "Conquest Map", 5, 5));
            maps.Add(new Map(GraphicPath + "/" + MapPath + "/" + "ArenaMap.png", "Arena Map", 5, 5));
            maps.Add(new Map(GraphicPath + "/" + MapPath + "/" + "JoustMap.png", "Joust Map", 3, 3));
            maps.Add(new Map(GraphicPath + "/" + MapPath + "/" + "SiegeMap.png", "Siege Map", 5, 5));
            maps.Add(new Map(GraphicPath + "/" + MapPath + "/" + "AssaultMap.png", "Assault Map", 5, 5));

            maps[0].TurretList.Add(new TurretInstance(0, 0, 0));
            maps[0].TurretList.Add(new TurretInstance(50, 0, 0));
            maps[0].TurretList.Add(new TurretInstance(100, 0, 2));
            maps[0].TurretList.Add(new TurretInstance(150, 0, 1));
            maps[0].TurretList.Add(new TurretInstance(200, 0, 1));
            maps[0].TurretList.Add(new TurretInstance(250, 0, 3));

            turrets.Add(new Turret(GraphicPath + "/" + TurretPath + "/" + "BlueTurret.png", "Blue Turret"));
            turrets.Add(new Turret(GraphicPath + "/" + TurretPath + "/" + "RedTurret.png", "Red Turret"));
            turrets.Add(new Turret(GraphicPath + "/" + TurretPath + "/" + "BluePhoenix.png", "Blue Phoenix"));
            turrets.Add(new Turret(GraphicPath + "/" + TurretPath + "/" + "RedPhoenix.png", "Red Phoenix"));

            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Agni.png", "Agni", 132, 104, 153, 137));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Ah Muzen Cab.png", "Ah Muzen Cab", 162, 117, 184, 157));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Ah Puch.png", "Ah Puch", 213, 90, 244, 121));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Anhur.png", "Anhur", 156, 86, 181, 119));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Anubis.png", "Anubis", 119, 38, 156, 78));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Ao Kuang.png", "Ao Kuang", 164, 80, 200, 112));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Aphrodite.png", "Aphrodite", 197, 58, 223, 92));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Apollo.png", "Apollo", 200, 75, 220, 114));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Arachne.png", "Arachne", 94, 39, 124, 79));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Ares.png", "Ares", 107, 187, 139, 227));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Artemis.png", "Artemis", 107, 56, 145, 96));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Athena.png", "Athena", 51, 49, 80, 82));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Awilix.png", "Awilix", 172, 37, 191, 72));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Bacchus.png", "Bacchus", 126, 73, 154, 102));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Bakasura.png", "Bakasura", 202, 50, 217, 80));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Bastet.png", "Bastet", 130, 65, 149, 101));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Bellona.png", "Bellona", 171, 58, 196, 84));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Cabraken.png", "Cabraken", 147, 129, 172, 196));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Chaac.png", "Chaac", 107, 71, 131, 101));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Chang'e.png", "Chang'e", 186, 52, 217, 88));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Chronos.png", "Chronos", 124, 58, 148, 78));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Cupid.png", "Cupid", 136, 165, 144, 203));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Fenrir.png", "Fenrir", 51, 83, 73, 104));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Freya.png", "Freya", 130, 29, 177, 62));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Geb.png", "Geb", 56, 142, 85, 184));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Guan Yu.png", "Guan Yu", 131, 58, 160, 90));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Hades.png", "Hades", 132, 126, 162, 150));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "He Bo.png", "He Bo", 104, 83, 139, 122));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Hel.png", "Hel", 139, 65, 164, 96));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Hercules.png", "Hercules", 143, 52, 162, 75));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Hou Yi.png", "Hou Yi", 140, 69, 164, 114));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Hun Batz.png", "Hun Batz", 248, 214, 286, 229));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Isis.png", "Isis", 148, 129, 172, 166));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Janus.png", "Janus", 159, 39, 185, 73));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Kali.png", "Kali", 145, 77, 171, 116));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Kukulkan.png", "Kukulkan", 153, 216, 152, 215));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Kumbhakarna.png", "Kumbhakarna", 166, 91, 187, 129));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Loki.png", "Loki", 100, 95, 116, 123));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Medusa.png", "Medusa", 99, 103, 124, 138));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Mercury.png", "Mercury", 97, 120, 115, 144));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Ne Zha.png", "Ne Zha", 134, 57, 156, 81));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Neith.png", "Neith", 122, 40, 148, 76));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Nemesis.png", "Nemesis", 155, 97, 182, 134));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Nox.png", "Nox", 181, 80, 201, 108));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Nu Wa.png", "Nu Wa", 96, 59, 121, 86));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Odin.png", "Odin", 138, 37, 162, 67));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Osiris.png", "Osiris", 147, 109, 174, 144));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Poseidon.png", "Poseidon", 151, 114, 173, 140));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Ra.png", "Ra", 146, 37, 173, 69));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Rama.png", "Rama", 166, 61, 186, 91));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Scylla.png", "Scylla", 171, 162, 198, 195));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Serqet.png", "Serqet", 223, 21, 250, 51));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Sobek.png", "Sobek", 117, 121, 131, 136));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Sun Wukong.png", "Sun Wukong", 133, 99, 160, 136));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Sylvanus.png", "Sylvanus", 146, 120, 174, 147));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Thanatos.png", "Thanatos", 122, 87, 148,115));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Thor.png", "Thor", 116, 17, 151, 42));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Tyr.png", "Tyr", 217, 53, 246, 78));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Ullr.png", "Ullr", 133, 34, 166, 64));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Vamana.png", "Vamana", 68, 76, 87, 103));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Vulcan.png", "Vulcan", 146, 106, 170, 133));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Xbalanque.png", "Xbalanque", 79, 84, 102, 108));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Ymir.png", "Ymir", 118, 64, 144, 83));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Zeus.png", "Zeus", 150, 117, 173, 146));
            gods.Add(new God(GraphicPath + "/" + GodPath + "/" + "Zhong Kui.png", "Zhong Kui", 195, 89, 222, 115));

            miscanellous.Add(new Graphics(GraphicPath + "/" + MiscanellousPath + "/" + "FaceSelector.png", "Face Selector"));
            miscanellous.Add(new Graphics(GraphicPath + "/" + MiscanellousPath + "/" + "MiniSelector.png", "Mini Selector"));
        }
    }
}
