﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model
{
    public class SkillBasicInformation : IXmlConvertible, IXmlStateRecoverable
    {
        public Sprite Image
        {
            get
            {
                return ResourceLoadUtility.LoadIcon(IconName);
            }
        }

        public string IconName
        {
            get; set;
        }

        public SkillType Type
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int AcquisitionLevel
        {
            get; set;
        }

        public int MaximumLevel
        {
            get; set;
        }

        public int RequiredUpgradeCost
        {
            get; set;
        }

        public void RecoverStateFromXml(string rawXml)
        {
            var element = XElement.Parse(rawXml);

            IconName = element.AttributeValue("Icon");
            Type = (SkillType) Enum.Parse(typeof(SkillType), element.AttributeValue("Type").ToString());
            Name = element.AttributeValue("Name");
            AcquisitionLevel = element.AttributeValue("AcquisitionLevel", int.Parse);
            MaximumLevel = element.AttributeValue("MaximumLevel", int.Parse);
            RequiredUpgradeCost = element.AttributeValue("RequiredUpgradeCost", int.Parse);
        }

        public XElement ToXmlElement()
        {
            return new XElement("SkillInfo",
                new XAttribute("Icon", IconName),
                new XAttribute("Type", Type.ToString()),
                new XAttribute("Name", Name),
                new XAttribute("AcquisitionLevel", AcquisitionLevel),
                new XAttribute("MaximumLevel", MaximumLevel),
                new XAttribute("RequiredUpgradeCost", RequiredUpgradeCost));
        }
    }
}
