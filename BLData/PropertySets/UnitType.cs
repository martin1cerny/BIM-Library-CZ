﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of unit type.
    /// </summary>
    public class UnitType : BLEntity
    {
        private UnitTypeEnum? _type;

        /// <summary>
        /// The unit type based on IfcDerivedUnit(IfcDerivedUnitEnum) and IfcNamedUnit(IfcUnitEnum) of IFC4 Official Release.
        /// </summary>
        [XmlAttribute("type")]
        [DefaultValue("USERDEFINED")]
        public string _Value 
        {
            get
            {
                return _type.ToString();
            }
            set
            {
                if (_model != null) throw new InvalidOperationException();

                UnitTypeEnum type = UnitTypeEnum.USERDEFINED;
                if (String.IsNullOrEmpty(value))
                    _type = null;
                else if (Enum.TryParse<UnitTypeEnum>(value, out type))
                {
                    _type = type;
                }
                else
                    throw new ArgumentOutOfRangeException(value);
        
            }
        }
        
        [XmlIgnore]
        public UnitTypeEnum? Type { 
            get { return _type; }
            set { var old = _type; Set("Type", () => _type = value, () => _type = old); } 
        }


        private string _currencyType;

        [XmlAttribute("currencytype")]
        [DefaultValue("USERDEFINED")]
        public string CurrencyType {
            get { return _currencyType; }
            set { var old = _currencyType; Set("CurrencyType", () => _currencyType = value, () => _currencyType = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            return "";
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }

    public enum UnitTypeEnum
    {
        USERDEFINED,
        ACCELERATIONUNIT,
        ANGULARVELOCITYUNIT,
        AREADENSITYUNIT,
        COMPOUNDPLANEANGLEUNIT,
        DYNAMICVISCOSITYUNIT,
        HEATFLUXDENSITYUNIT,
        INTEGERCOUNTRATEUNIT,
        ISOTHERMALMOISTURECAPACITYUNIT,
        KINEMATICVISCOSITYUNIT,
        LINEARFORCEUNIT,
        LINEARMOMENTUNIT,
        LINEARSTIFFNESSUNIT,
        LINEARVELOCITYUNIT,
        MASSDENSITYUNIT,
        MASSFLOWRATEUNIT,
        MODULUSOFELASTICITYUNIT,
        MODULUSOFSUBGRADEREACTIONUNIT,
        MOISTUREDIFFUSIVITYUNIT,
        MOLECULARWEIGHTUNIT,
        MOMENTORINERTIAUNIT,
        PLANARFORCEUNIT,
        ROTATIONALFREQUENCYUNIT,
        ROTATIONALSTIFFNESSUNIT,
        SHEARMODULUSUNIT,
        SPECIFICHEATCAPACITYUNIT,
        THERMALADMITTANCEUNIT,
        THERMALCONDUCTANCEUNIT,
        THERMALRESISTANCEUNIT,
        THERMALTRANSMITTANCEUNIT,
        TORQUEUNIT,
        VAPORPERMEABILITYUNIT,
        VOLUMETRICFLOWRATEUNIT,
        CURVATUREUNIT,
        HEATINGVALUEUNIT,
        IONCONCENTRATIONUNIT,
        LUMINOUSINTENSITYDISTRIBUTIONUNIT,
        MASSPERLENGTHUNIT,
        MODULUSOFLINEARSUBGRADEREACTIONUNIT,
        MODULUSOFROTATIONALSUBGRADEREACTIONUNIT,
        PHUNIT,
        ROTATIONALMASSUNIT,
        SECTIONAREAINTEGRALUNIT,
        SECTIONMODULUSUNIT,
        SOUNDPOWERLEVELUNIT,
        SOUNDPOWERUNIT,
        SOUNDPRESSURELEVELUNIT,
        SOUNDPRESSUREUNIT,
        TEMPERATUREGRADIENTUNIT,
        TEMPERATURERATEOFCHANGEUNIT,
        THERMALEXPANSIONCOEFFICIENTUNIT,
        WARPINGCONSTANTUNIT,
        WARPINGMOMENTUNIT,
        ABSORBEDDOSEUNIT,
        AMOUNTOFSUBSTANCEUNIT,
        AREAUNIT,
        DOSEEQUIVALENTUNIT,
        ELECTRICCAPACITANCEUNIT,
        ELECTRICCHARGEUNIT,
        ELECTRICCONDUCTANCEUNIT,
        ELECTRICCURRENTUNIT,
        ELECTRICRESISTANCEUNIT,
        ELECTRICVOLTAGEUNIT,
        ENERGYUNIT,
        FORCEUNIT,
        FREQUENCYUNIT,
        ILLUMINANCEUNIT,
        INDUCTANCEUNIT,
        LENGTHUNIT,
        LUMINOUSFLUXUNIT,
        LUMINOUSINTENSITYUNIT,
        MAGNETICFLUXDENSITYUNIT,
        MAGNETICFLUXUNIT,
        MASSUNIT,
        PLANEANGLEUNIT,
        POWERUNIT,
        PRESSUREUNIT,
        RADIOACTIVITYUNIT,
        SOLIDANGLEUNIT,
        THERMODYNAMICTEMPERATUREUNIT,
        TIMEUNIT,
        VOLUMEUNIT,

        [Obsolete]
        IFCMONETARYUNIT,
        [Obsolete]
        UNSPECIFIED
    }
}
