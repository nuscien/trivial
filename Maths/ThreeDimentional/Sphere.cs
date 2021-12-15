using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Maths
{
    /// <summary>
    /// The sphere in coordinate.
    /// </summary>
    [DataContract]
    public class CoordinateSphere : ICloneable
    {
        private DoubleThreeDimensionalPoint center;

        /// <summary>
        /// Initializes a new instance of the CoordinateSphere class.
        /// </summary>
        public CoordinateSphere()
        {
            center = new();
            Radius = 0;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateSphere class.
        /// </summary>
        /// <param name="x">The x of center point.</param>
        /// <param name="y">The y of center point.</param>
        /// <param name="z">The z of center point.</param>
        /// <param name="r">The radius.</param>
        public CoordinateSphere(double x, double y, double z, double r)
        {
            center = new(x, y, z);
            Radius = double.IsNaN(r) ? 0 : r;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateSphere class.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="r">The radius.</param>
        public CoordinateSphere(DoubleThreeDimensionalPoint center, double r)
        {
            this.center = center ?? new();
            Radius = double.IsNaN(r) ? 0 : r;
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        [JsonPropertyName("r")]
        [DataMember(Name = "r")]
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the center point.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DoubleThreeDimensionalPoint Center
        {
            get => center;
            set => center = value ?? new();
        }

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("x")]
        [DataMember(Name = "x")]
        public double X
        {
            get => Center.X;
            set => Center.X = value;
        }

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("y")]
        [DataMember(Name = "y")]
        public double Y
        {
            get => Center.Y;
            set => Center.Y = value;
        }

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("z")]
        [DataMember(Name = "z")]
        public double Z
        {
            get => Center.Z;
            set => Center.Z = value;
        }

        /// <summary>
        /// Gets the area.
        /// </summary>
        public double Area()
            => Math.PI * Radius * Radius * 4;

        /// <summary>
        /// Gets the perimeter.
        /// </summary>
        public double Volume()
            => Math.PI * Math.Pow(Radius, 3) * 4 / 3;

        /// <summary>
        /// Returns a string that represents the line.
        /// </summary>
        /// <returns>A string that represents the line.</returns>
        public override string ToString()
        {
            try
            {
                return $"{X:0.########}, {Y:0.########}, {Z:0.########} (r {Radius:0.########})";
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (FormatException)
            {
            }
            catch (NullReferenceException)
            {
            }

            return $"{X}, {Y}, {Z} (r {Radius})";
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>An instance copied from current one.</returns>
        public CoordinateSphere Clone()
            => new(center, double.IsNaN(Radius) ? 0d : Radius);

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>An instance copied from current one.</returns>
        object ICloneable.Clone()
            => Clone();
    }
}
