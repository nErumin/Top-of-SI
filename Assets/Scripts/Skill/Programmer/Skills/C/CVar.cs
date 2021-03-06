﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Model
{
    public class CVar : ActiveSkill, ISoundProducible
    {
        private static SkillBasicInformation information = new SkillBasicInformation
        {
            Name = "CVar",
            Type = SkillType.C,
            AcquisitionLevel = 0,
            MaximumLevel = 100,
            IconName = "C", 
            RequiredUpgradeCost = 3
        };

        public CVar()
            : base(information, new List<PassiveSkill>() { new NoteDown(), new CounterEvolution() }, 5)
        {
            Accuracy = 0.9;
        }

        public AudioClip EffectSound
        {
            get
            {
                return ResourceLoadUtility.LoadEffectClip("Cannon");
            }
        }

        public override void LevelUP()
        {
            Information.AcquisitionLevel++;
            if(Information.AcquisitionLevel == 10)
            {
                FlattenContainingPassiveSkills().Where(x => x.Information.Name.Equals("NoteDown")).ToArray()[0].EnableToLearn();
            }
            else if(Information.AcquisitionLevel == 40)
            {
                FlattenContainingPassiveSkills().Where(x => x.Information.Name.Equals("CounterEvolution")).ToArray()[0].EnableToLearn();
            }
        }

        protected override double CalculateSkillLevelDamage(double projectTypeAppliedDamage)
        {
            return projectTypeAppliedDamage * Information.AcquisitionLevel / 2;
        }
    }
}
