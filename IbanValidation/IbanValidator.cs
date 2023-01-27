using System.Numerics;
using System.Text.RegularExpressions;

namespace IbanValidation;

public class IbanValidator
{
    /// <summary>
    /// Dictionary containing country codes and their corresponding IBAN lengths
    /// </summary>
    private Dictionary<string, int> IbanLengths = new()
    {
        { "AD" , 24 }, { "AE" , 23 }, { "AL" , 28 }, { "AO" , 25 }, { "AT" , 20 },
        { "AX" , 18 }, { "AZ" , 28 }, { "BA" , 20 }, { "BE" , 16 }, { "BF" , 28 },
        { "BG" , 22 }, { "BH" , 22 }, { "BI" , 27 }, { "BJ" , 28 }, { "BL" , 27 },
        { "BR" , 29 }, { "BY" , 28 }, { "CF" , 27 }, { "CG" , 27 }, { "CH" , 21 },
        { "CI" , 28 }, { "CM" , 27 }, { "CR" , 22 }, { "CV" , 25 }, { "CY" , 28 },
        { "CZ" , 24 }, { "DE" , 22 }, { "DJ" , 27 }, { "DK" , 18 }, { "DO" , 28 },
        { "DZ" , 26 }, { "EA" , 24 }, { "EE" , 20 }, { "EG" , 29 }, { "ES" , 24 },
        { "FI" , 18 }, { "FO" , 18 }, { "FR" , 27 }, { "GA" , 27 }, { "GB" , 22 },
        { "GE" , 22 }, { "GF" , 27 }, { "GG" , 22 }, { "GI" , 23 }, { "GL" , 18 },
        { "GP" , 27 }, { "GQ" , 27 }, { "GR" , 27 }, { "GT" , 28 }, { "GW" , 25 },
        { "HN" , 28 }, { "HR" , 21 }, { "HU" , 28 }, { "IC" , 24 }, { "IE" , 22 },
        { "IL" , 23 }, { "IM" , 22 }, { "IQ" , 23 }, { "IR" , 26 }, { "IS" , 26 },
        { "IT" , 27 }, { "JE" , 22 }, { "JO" , 30 }, { "KM" , 27 }, { "KW" , 30 },
        { "KZ" , 20 }, { "LB" , 28 }, { "LC" , 32 }, { "LI" , 21 }, { "LT" , 20 },
        { "LU" , 20 }, { "LV" , 21 }, { "LY" , 25 }, { "MA" , 28 }, { "MC" , 27 },
        { "MD" , 24 }, { "ME" , 22 }, { "MF" , 27 }, { "MG" , 27 }, { "MK" , 19 },
        { "ML" , 28 }, { "MN" , 20 }, { "MQ" , 27 }, { "MR" , 27 }, { "MT" , 31 },
        { "MU" , 30 }, { "MZ" , 25 }, { "NC" , 27 }, { "NE" , 28 }, { "NI" , 32 },
        { "NL" , 18 }, { "NO" , 15 }, { "PF" , 27 }, { "PK" , 24 }, { "PL" , 28 },
        { "PM" , 27 }, { "PS" , 29 }, { "PT" , 25 }, { "QA" , 29 }, { "RE" , 27 },
        { "RO" , 24 }, { "RS" , 22 }, { "RU" , 33 }, { "SA" , 24 }, { "SC" , 31 },
        { "SD" , 18 }, { "SE" , 24 }, { "SI" , 19 }, { "SK" , 24 }, { "SM" , 27 },
        { "SN" , 28 }, { "ST" , 25 }, { "SV" , 28 }, { "TD" , 27 }, { "TF" , 27 },
        { "TG" , 28 }, { "TL" , 23 }, { "TN" , 24 }, { "TR" , 26 }, { "UA" , 29 },
        { "VA" , 22 }, { "VG" , 24 }, { "WF" , 27 }, { "XK" , 20 }, { "YT" , 27 }
    };

    /// <summary>
    /// Validates an IBAN's length. The length is determined based on the country
    /// code at the beginning of the IBAN.
    /// </summary>
    /// <param name="iban">The IBAN to validate. Should not have any invalid characters</param>
    /// <returns>`true` if the length is valid. `false` if the length is invalid.</returns>
    private bool ValidateLength(string iban)
    {
        // If we don't even have enough characters to determine the country,
        // then it's definitely invalid
        if (iban.Length < 2)
            return false;

        // Get the country code and use it for validation
        var countryCode = iban.Substring(0, 2).ToUpper();

        if (!IbanLengths.ContainsKey(countryCode) ||
            iban.Length != IbanLengths[countryCode])
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates an IBAN using the checksum and MOD 97 algorithm.
    /// </summary>
    /// <param name="iban">The IBAN to validate. Should not have any invalid characters</param>
    /// <returns>`tru` if the checksum is valid. `false` if the checksum is invalid.</returns>
    private bool ValidateChecksum(string iban)
    {
        // First rearrange the IBAN so that the first four characters
        // are at the end of the IBAN
        var rearrangedIban = iban.Substring(4) + iban.Substring(0, 4);

        // We need to make sure the IBAN is in one case, i.e. upper case
        rearrangedIban = rearrangedIban.ToUpper();

        // We need to convert all the characters into numbers so that we can
        // perform the mod operation on it. The ciphers variable will hold the
        // converted IBAN.
        // See the details below on how we do this.
        var ciphers = "";

        // Loop through each character of the IBAN and calculate the checksum
        // as we go
        foreach (var c in rearrangedIban)
        {
            // All characters from 0-9 have a value matching their number.
            // I.e. '0' = 0, '1' = 1, etc
            // All alphabet characters have a value based on their position in
            // the alphabet, starting from 10
            // I.e. 'A' = 10, 'B' = 11, etc
            // Here we convert the character into its numerical value and add it
            // to ciphers.
            if (char.IsNumber(c))
                ciphers += (c - '0').ToString();
            else
                ciphers += (c - 'A' + 10).ToString();
        }

        // Now that we have the IBAN as just numbers, we can % 97 it. To work with
        // such a large number, we use the BigInteger class.
        // A valid checksum is when the result of the mod == 1
        return BigInteger.Parse(ciphers) % 97 == 1;
    }

    /// <summary>
    /// Validates an IBAN using country-specific lengths and the checksum along with
    /// the MOD 97 algorithm.
    /// </summary>
    /// <param name="iban">The IBAN to validate. Allowed to contian spacing or similar characters for readability.</param>
    /// <returns>`true` if the IBAN is valid. `false` if the IBAN is invalid.</returns>
    public bool Validate(string iban)
    {
        // Remove any non-alphanumeric characters
        iban = Regex.Replace(iban, "[^0-9A-Z]*", "", RegexOptions.IgnoreCase);

        // Validate the IBAN length
        if (!ValidateLength(iban))
            return false;

        // Validate the IBAN checksum
        return ValidateChecksum(iban);
    }
}
