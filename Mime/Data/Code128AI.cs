using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Data
{
    /// <summary>
    /// The code-128, which is a high-density linear barcode symbology defined in ISO/IEC 15417:2007.
    /// It is used for alphanumeric or numeric-only barcodes.
    /// </summary>
    public partial class Code128
    {
#pragma warning disable IDE0056
        /// <summary>
        /// Commonly used GS1-128 generator which identifies data with Application Identifiers.
        /// </summary>
        public static class Gs1Generator
        {
            /// <summary>
            /// Creates serial shipping container code.
            /// </summary>
            /// <param name="data">The data without AI, length should be 18.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Sscc(string data)
                => CreateGs1(0, data);

            /// <summary>
            /// Creates global trade item number.
            /// </summary>
            /// <param name="data">The data without AI, length should be 14.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Gtin(string data)
                => CreateGs1(1, data);

            /// <summary>
            /// Creates global trade item number of contained trade items.
            /// </summary>
            /// <param name="data">The data without AI, length should be 14.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 GtinOfContainedTradeItems(string data)
                => CreateGs1(2, data);

            /// <summary>
            /// Creates batch number or lot number.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 20.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Batch(string data)
                => CreateGs1(10, data);

            /// <summary>
            /// Creates production date.
            /// </summary>
            /// <param name="data">The data without AI.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Production(DateTime data)
                => CreateGs1Date(11, data);

            /// <summary>
            /// Creates due date.
            /// </summary>
            /// <param name="data">The data without AI.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Due(DateTime data)
                => CreateGs1Date(12, data);

            /// <summary>
            /// Creates packaging date.
            /// </summary>
            /// <param name="data">The data without AI.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Packaging(DateTime data)
                => CreateGs1Date(13, data);

            /// <summary>
            /// Creates best before date.
            /// </summary>
            /// <param name="data">The data without AI.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 BestBefore(DateTime data)
                => CreateGs1Date(15, data);

            /// <summary>
            /// Creates expiration date.
            /// </summary>
            /// <param name="data">The data without AI.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Expiration(DateTime data)
                => CreateGs1Date(17, data);

            /// <summary>
            /// Creates product variant.
            /// </summary>
            /// <param name="data">The data without AI, length should be 2.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 ProductVariant(string data)
                => CreateGs1(20, data);

            /// <summary>
            /// Creates serial number.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 20.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Sn(string data)
                => CreateGs1(21, data);

            /// <summary>
            /// Creates secondary data fields.
            /// </summary>
            /// <param name="data">The data without AI, length should be 29.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 SecondaryData(string data)
                => CreateGs1(22, data);

            /// <summary>
            /// Creates additional product identification.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 AdditionalProductIdentification(string data)
                => CreateGs1(240, data);

            /// <summary>
            /// Creates customer part number.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 CustomerPart(string data)
                => CreateGs1(241, data);

            /// <summary>
            /// Creates Made-to-Order Variation Number.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 6.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 MadeToOrderVariation(string data)
                => CreateGs1(242, data);

            /// <summary>
            /// Creates packaging component number.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 20.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 PackagingComponent(string data)
                => CreateGs1(243, data);

            /// <summary>
            /// Creates secondary serial number.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Sn2(string data)
                => CreateGs1(250, data);

            /// <summary>
            /// Creates reference to source entity.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 SourceEntity(string data)
                => CreateGs1(251, data);

            /// <summary>
            /// Creates global document type identifier.
            /// </summary>
            /// <param name="data">The data without AI, length is variable from 13 to 17.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 DocumentTypeId(string data)
                => CreateGs1(253, data);

            /// <summary>
            /// Creates GLN extension component.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 20.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 GlnExtensionComponent(string data)
                => CreateGs1(254, data);

            /// <summary>
            /// Creates global coupon number.
            /// </summary>
            /// <param name="data">The data without AI, length is variable from 13 to 25.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Gcn(string data)
                => CreateGs1(255, data);

            /// <summary>
            /// Creates count of items.
            /// </summary>
            /// <param name="data">The data without AI, should be less than 1_000_000_000.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Count(int data)
                => CreateGs1(30, data.ToString("g"));

            /// <summary>
            /// Creates net weight in kilo gram (kg).
            /// </summary>
            /// <param name="data">The data without AI, should be less than 1,000,000,000.</param>
            /// <param name="container">true if it is for container; otherwise, false, for product.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Weight(int data, bool container = false)
                => CreateGs1(container ? 3300 : 3100, data.ToString("000000"));

            /// <summary>
            /// Creates size in meters.
            /// </summary>
            /// <param name="x">The length, should be less than 1,000,000.</param>
            /// <param name="y">The width/diamete, should be less than 1,000,000.</param>
            /// <param name="z">The depth/thickness/height, should be less than 1,000,000.</param>
            /// <param name="container">true if it is for container; otherwise, false, for product.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Size(int x, int? y = null, int? z = null, bool container = false)
            {
                var col = new List<byte> { 102, (byte)(container ? 33 : 31), 10 };
                Fill(col, Subtypes.C, x.ToString("000000"));
                if (y.HasValue)
                {
                    col.Add((byte)(container ? 33 : 31));
                    col.Add(20);
                    Fill(col, Subtypes.C, y.Value.ToString("000000"));
                }

                if (z.HasValue)
                {
                    col.Add((byte)(container ? 33 : 31));
                    col.Add(30);
                    Fill(col, Subtypes.C, z.Value.ToString("000000"));
                }

                return Create(Subtypes.C, col);
            }

            /// <summary>
            /// Creates area in square meters.
            /// </summary>
            /// <param name="data">The data without AI, should be less than 1,000,000,000.</param>
            /// <param name="container">true if it is for container; otherwise, false, for product.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Area(int data, bool container = false)
                => CreateGs1(container ? 3340 : 3140, data.ToString("000000"));

            /// <summary>
            /// Creates net volume in liters.
            /// </summary>
            /// <param name="data">The data without AI, should be less than 1,000,000,000.</param>
            /// <param name="container">true if it is for container; otherwise, false, for product.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Volume(int data, bool container = false)
                => CreateGs1(container ? 3350 : 3150, data.ToString("000000"));

            /// <summary>
            /// Creates net volume in liters.
            /// </summary>
            /// <param name="kind">
            /// <para>The kind of property.</para>
            /// <list type="bullet">
            /// <item>Net weight in kg (0-9).</item>
            /// <item>Length in meters (10-19).</item>
            /// <item>Width/diamete in meters (20-29).</item>
            /// <item>Depth/thickness/height in meters (30-39).</item>
            /// <item>Area in square meters (40-49).</item>
            /// <item>Volume in liters (50-59), or in cubic meters (60-69).</item>
            /// </list>
            /// </param>
            /// <param name="data">The data without AI, should be less than 1,000,000,000.</param>
            /// <param name="container">true if it is for container; otherwise, false, for product.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Property(byte kind, int data, bool container = false)
                => CreateGs1((container ? 33 : 31) * 100 + kind, data.ToString("000000"));

            /// <summary>
            /// Creates count of units contained.
            /// </summary>
            /// <param name="data">The data without AI, should be less than 1_000_000_000.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 UnitCount(int data)
                => CreateGs1(37, data.ToString("g"));

            /// <summary>
            /// Creates net volume in liters.
            /// </summary>
            /// <param name="kind">
            /// <para>The kind of property.</para>
            /// <list type="bullet">
            /// <item>Local currency (0-9).</item>
            /// <item>With ISO currency code (10-19).</item>
            /// <item>Local currency per single item (20-29).</item>
            /// <item>With ISO currency code per single item (30-39).</item>
            /// </list>
            /// </param>
            /// <param name="data">The data without AI, should be less than 1,000,000,000.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 AmountPayable(byte kind, string data)
                => CreateGs1(39 * 100 + kind, data);

            /// <summary>
            /// Creates Customer purchase order number.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Order(string data)
                => CreateGs1(400, data);

            /// <summary>
            /// Creates consignment number.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Consignment(string data)
                => CreateGs1(401, data);

            /// <summary>
            /// Creates bill of lading number.
            /// </summary>
            /// <param name="data">The data without AI, length should be 17.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 LadingBill(string data)
                => CreateGs1(402, data);

            /// <summary>
            /// Creates routing code.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Routing(string data)
                => CreateGs1(403, data);

            /// <summary>
            /// Creates NATO stock number.
            /// </summary>
            /// <param name="data">The data without AI, length should be 13.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 Nsn(string data)
                => CreateGs1(7001, data);

            /// <summary>
            /// Creates active potency.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 4.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 ActivePotency(string data)
                => CreateGs1(7004, data);

            /// <summary>
            /// Creates roll products: width/length/core diameter/direction/splices.
            /// </summary>
            /// <param name="data">The data without AI, length should be 14.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 RollProducts(string data)
                => CreateGs1(8001, data);

            /// <summary>
            /// Creates mobile phone identifier.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 20.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 PhoneId(string data)
                => CreateGs1(8002, data);

            /// <summary>
            /// Creates global returnable asset identifier.
            /// </summary>
            /// <param name="data">The data without AI, length is variable from 14 to 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 GlobalReturnableAsset(string data)
                => CreateGs1(8003, data);

            /// <summary>
            /// Creates global individual asset identifier.
            /// </summary>
            /// <param name="data">The data without AI, length should be 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 GlobalIndividualAsset(string data)
                => CreateGs1(8004, data);

            /// <summary>
            /// Creates international bank account number.
            /// </summary>
            /// <param name="data">The data without AI, length should be 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 BankAccount(string data)
                => CreateGs1(8007, data);

            /// <summary>
            /// Creates global service relationship number.
            /// </summary>
            /// <param name="data">The data without AI, length is variable from 13 to 25.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 GlobalServiceRelationship(string data)
                => CreateGs1(8018, data);

            /// <summary>
            /// Creates payment slip reference number.
            /// </summary>
            /// <param name="data">The data without AI, length is variable from 13 to 25.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 PaymentSlip(string data)
                => CreateGs1(8020, data);

            /// <summary>
            /// Creates extended packaging URL.
            /// </summary>
            /// <param name="data">The data without AI, length tp to 70.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 ExtendedPackagingURL(string data)
                => CreateGs1(8200, data);

            /// <summary>
            /// Creates mutually agreed between trading partners.
            /// </summary>
            /// <param name="data">The data without AI, length is up to 30.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 MutuallyAgreedBetweenTradingPartners(string data)
                => CreateGs1(90, data);

            /// <summary>
            /// Creates internal company codes (AI 91).
            /// </summary>
            /// <param name="data">The data without AI, length is up to 90.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 InternalCompanyCodes1(string data)
                => CreateGs1(91, data);

            /// <summary>
            /// Creates internal company codes (AI 92).
            /// </summary>
            /// <param name="data">The data without AI, length is up to 90.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 InternalCompanyCodes2(string data)
                => CreateGs1(92, data);

            /// <summary>
            /// Creates internal company codes (AI 93).
            /// </summary>
            /// <param name="data">The data without AI, length is up to 90.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 InternalCompanyCodes3(string data)
                => CreateGs1(93, data);

            /// <summary>
            /// Creates internal company codes (AI 94).
            /// </summary>
            /// <param name="data">The data without AI, length is up to 90.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 InternalCompanyCodes4(string data)
                => CreateGs1(94, data);

            /// <summary>
            /// Creates internal company codes (AI 95).
            /// </summary>
            /// <param name="data">The data without AI, length is up to 90.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 InternalCompanyCodes5(string data)
                => CreateGs1(95, data);

            /// <summary>
            /// Creates internal company codes (AI 96).
            /// </summary>
            /// <param name="data">The data without AI, length is up to 90.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 InternalCompanyCodes6(string data)
                => CreateGs1(96, data);

            /// <summary>
            /// Creates internal company codes (AI 97).
            /// </summary>
            /// <param name="data">The data without AI, length is up to 90.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 InternalCompanyCodes7(string data)
                => CreateGs1(97, data);

            /// <summary>
            /// Creates internal company codes (AI 98).
            /// </summary>
            /// <param name="data">The data without AI, length is up to 90.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 InternalCompanyCodes8(string data)
                => CreateGs1(98, data);

            /// <summary>
            /// Creates internal company codes (AI 99).
            /// </summary>
            /// <param name="data">The data without AI, length is up to 90.</param>
            /// <returns>The code 128 instance.</returns>
            public static Code128 InternalCompanyCodes9(string data)
                => CreateGs1(99, data);
        }
#pragma warning restore IDE0056

        private static Code128 CreateGs1Date(int ai, DateTime date)
            => CreateGs1(ai, date.ToString("yyMMdd"));
    }
}
