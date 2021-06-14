using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Geography
{
    /// <summary>
    /// Topographies.
    /// </summary>
    public enum Topographies
    {
        /// <summary>
        /// Plain topography.
        /// </summary>
        Plain = 0,

        /// <summary>
        /// Plateau topography.
        /// </summary>
        Plateau = 1,

        /// <summary>
        /// Hills topography.
        /// </summary>
        Hills = 2,

        /// <summary>
        /// Basin topography.
        /// </summary>
        Basin = 3,

        /// <summary>
        /// Mountain topography.
        /// </summary>
        Mountain = 4
    }

    /// <summary>
    /// The seven continents on Earth.
    /// </summary>
    public enum Continents
    {
        /// <summary>
        /// Asia.
        /// </summary>
        Asia = 0,

        /// <summary>
        /// Europe.
        /// </summary>
        Europe = 1,

        /// <summary>
        /// North America.
        /// </summary>
        NorthAmerica = 2,

        /// <summary>
        /// South America.
        /// </summary>
        SouthAmerica = 3,

        /// <summary>
        /// Africa.
        /// </summary>
        Africa = 4,

        /// <summary>
        /// Oceania.
        /// </summary>
        Oceania = 5,

        /// <summary>
        /// Antarctica.
        /// </summary>
        Antarctica = 6
    }

    /// <summary>
    /// The four oceans on Earth.
    /// </summary>
    public enum Oceans
    {
        /// <summary>
        /// The Pacific Ocean.
        /// </summary>
        Pacific = 0,

        /// <summary>
        /// The Atlantic Ocean.
        /// </summary>
        Atlantic = 1,

        /// <summary>
        /// The Indian Ocean.
        /// </summary>
        Indian = 2,

        /// <summary>
        /// The Arctic  Ocean.
        /// </summary>
        Arctic = 3
    }

    /// <summary>
    /// The solar climatic zone.
    /// </summary>
    public enum SolarClimaticZones
    {
        /// <summary>
        /// Tropics.
        /// </summary>
        Tropics = 0,

        /// <summary>
        /// North temperate zone.
        /// </summary>
        NorthTemperateZone = 1,

        /// <summary>
        /// Sourth temperate zone.
        /// </summary>
        SouthTemperateZone = 2,

        /// <summary>
        /// The north frigid zone.
        /// </summary>
        NorthFrigidZone = 3,

        /// <summary>
        /// The south frigid zone.
        /// </summary>
        SouthFrigidZone = 4
    }

    /// <summary>
    /// The climate zones.
    /// </summary>
    public enum BuildingsClimateZones
    {
        /// <summary>
        /// The hot-humid climate.
        /// A region that receives more than 50 cm of annual precipitation and where one or both:
        /// A 19.5°C or higher wet bulb temperature for 3,000 or more hours during the warmest 6 consecutive months of the year; or
        /// A 23°C or higher wet bulb temperature for 1,500 or more hours during the warmest 6 consecutive months of the year.
        /// </summary>
        HotHumid = 0,

        /// <summary>
        /// The mixed-humid climate.
        /// A region that receives more than 50 cm of annual precipitation, has approximately 5,400 heating degree days or fewer,
        /// and where the average monthly outdoor temperature drops below 7°C during the winter months.
        /// </summary>
        MixedHumid = 1,

        /// <summary>
        /// The hot-dry climate.
        /// A region that receives less than 50 cm of annual precipitation and where the monthly average outdoor temperature remains above 7°C throughout the year.
        /// </summary>
        HotDry = 2,

        /// <summary>
        /// The mixed-dry climate.
        /// A region that receives less than 50 cm of annual precipitation, has approximately 5,400 heating degree days or less,
        /// and where the average monthly outdoor temperature drops below 7°C during the winter months.
        /// </summary>
        MixedDry = 3,

        /// <summary>
        /// The cold climate.
        /// A region with approximately 5,400 heating degree days or more and fewer than approximately 9,000 heating degree days.
        /// </summary>
        Cold = 4,

        /// <summary>
        /// The very-cold climate.
        /// A region with approximately 9,000 heating degree days or more and fewer than approximately 12,600 heating degree days.
        /// </summary>
        VeryCold = 5,

        /// <summary>
        /// The subarctic climate.
        /// A region with approximately 12,600 heating degree days or more.
        /// </summary>
        Subarctic = 6,

        /// <summary>
        /// The marine climate.
        /// A region that meets all of the following criteria:
        /// A mean temperature of coldest month between -3°C and 18°C;
        /// A warmest month mean of less than 22°C;
        /// At least 4 months with mean temperatures more than 10°C; and
        /// A dry season in summer.
        /// </summary>
        Marine = 7
    }
}
