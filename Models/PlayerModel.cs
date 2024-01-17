using System.Runtime.Serialization;

namespace sm_coding_challenge.Models
{
    [DataContract]
    public class PlayerModel
    {
        [DataMember(Name = "player_id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "position")]
        public string Position { get; set; }

        [DataMember(Name = "entry_id")]
        public string EntryId { get; set; }

        [DataMember(Name = "extra_pt_att")]
        public int ExtraPt { get; set; }

        [DataMember(Name = "extra_pt_made")]
        public int ExtraPtMade { get; set; }

        [DataMember(Name = "fld_goals_att")]
        public int FldGoals { get; set; }

        [DataMember(Name = "fld_goals_made")]
        public int FldGoalsMade { get; set; }
    }
}

