using MathNet.Numerics.Distributions;
using static System.Math;

namespace BlackScholesMertonAssetPricingModel;

/*
    NOTE: "MathNet.Numerics" is required for the BlackScholes formula
    calculation, you can install it using:
    "dotnet add package MathNet.Numerics"

    MathNet.Numerics is an open source library: https://github.com/mathnet/mathnet-numerics
*/

/// <summary>
/// <para>Base struct for the Black-Scholes-Merton formula calculations, no memory overhead is allocated.</para>
/// <para>Example:</para>
/// <para>var greeks = BlackScholes.Greeks(...); &lt;-- actual calculation</para>
/// <para>var delta = greeks.delta;</para>
/// <para>var gamma = greeks.gamma;</para>
/// </summary>
public readonly ref struct BlackScholes
{
    /* Normal Cumulative Distribution and Derivative calculation */
    private static readonly Normal N = new();
    private static double NCD(double x) => N.CumulativeDistribution(x);
    private static double Nderivative(double d) => (1 / Sqrt(2 * PI)) * Exp((-d * d) / 2);


    /*
        S = underlying's price (often denominated as "S0")
        K = option's strike price (often denominated as "X")
        v = implied volatility
        r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")
        q = dividend yield (makes for a more precise model)
        t = days to expiration (often denominated as "T - t")

        NOTE: percentages should be passed as e.g. 0.0310 for 3.10%
    */

    /// <summary>
    /// <para>Get the PRICE of an option contract.</para>
    /// <para>S = underlying's price (often denominated as "S0")</para>
    /// <para>K = option's strike price (often denominated as "X")</para>
    /// <para>v = implied volatility</para>
    /// <para>r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")</para>
    /// <para>q = dividend yield (makes for a more precise model)</para>
    /// <para>t = days to expiration (often denominated as "T - t")</para>
    /// <para>NOTE: percentages should be passed as e.g. 0.0310 for 3.10%</para>
    /// </summary>
    public static double Price(double S, double K, double v, double r, double q, double t, bool isCall, bool isCalendarDays = true, double d1 = 0, double d2 = 0)
    {
        /* "t" is actually the % of year to expiration, 
           the function takes the n. of days to expiration as input for simplicity
        
           by default n. of calendar days is used, although n. of trading days is viable as well
        */
        t /= isCalendarDays ? 365 : 252;

        d1 = d1 != 0 ? d1 : (Log(S / K) + t * (r - q + v * v / 2)) / (v * Sqrt(t));
        d2 = d2 != 0 ? d2 : d1 - v * Sqrt(t);

#if DEBUG
        var call = S * Exp(-q * t) * NCD(d1) - K * Exp(-r * t) * NCD(d2);
        var put = K * Exp(-r * t) * NCD(-d2) - S * Exp(-q * t) * NCD(-d1);
#endif

        return isCall ? S * Exp(-q * t) * NCD(d1) - K * Exp(-r * t) * NCD(d2) :
                        K * Exp(-r * t) * NCD(-d2) - S * Exp(-q * t) * NCD(-d1);
    }


    /* "gamma" and "vega" use the same formula for calls and puts, 
       "delta", "theta" and "rho" are different.
    */


    /* Delta: option price change per 1$ change in the underlying's price */
    /// <summary>
    /// <para>Delta: option price change per 1$ change in the underlying's price.</para>
    /// <para>Get the DELTA of an option contract.</para>
    /// <para>S = underlying's price (often denominated as "S0")</para>
    /// <para>K = option's strike price (often denominated as "X")</para>
    /// <para>v = implied volatility</para>
    /// <para>r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")</para>
    /// <para>q = dividend yield (makes for a more precise model)</para>
    /// <para>t = days to expiration (often denominated as "T - t")</para>
    /// <para>NOTE: percentages should be passed as e.g. 0.0310 for 3.10%</para>
    /// </summary>
    public static double Delta(double S, double K, double v, double r, double q, double t, bool isCall, bool isCalendarDays = true, double d1 = 0, double d2 = 0)
    {
        t /= isCalendarDays ? 365 : 252;

        d1 = d1 != 0 ? d1 : (Log(S / K) + t * (r - q + v * v / 2)) / (v * Sqrt(t));

#if DEBUG
        var call = Exp(-q * t) * NCD(d1);
        var put = -Exp(-q * t) * NCD(-d1);
#endif

        return isCall ? Exp(-q * t) * NCD(d1) :
                        -Exp(-q * t) * NCD(-d1);
    }

    /* Gamma: Delta change per 1$ change in the underlying's price */
    /// <summary>
    /// <para>Gamma: Delta change per 1$ change in the underlying's price.</para>
    /// <para>Get the GAMMA of an option contract.</para>
    /// <para>S = underlying's price (often denominated as "S0")</para>
    /// <para>K = option's strike price (often denominated as "X")</para>
    /// <para>v = implied volatility</para>
    /// <para>r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")</para>
    /// <para>q = dividend yield (makes for a more precise model)</para>
    /// <para>t = days to expiration (often denominated as "T - t")</para>
    /// <para>NOTE: percentages should be passed as e.g. 0.0310 for 3.10%</para>
    /// </summary>
    public static double Gamma(double S, double K, double v, double r, double q, double t, bool isCall, bool isCalendarDays = true, double d1 = 0, double d2 = 0)
    {
        t /= isCalendarDays ? 365 : 252;
        d1 = d1 != 0 ? d1 : (Log(S / K) + t * (r - q + v * v / 2)) / (v * Sqrt(t));

        return (Exp(-q * t) / (S * v * Sqrt(t))) * Nderivative(d1);
    }

    /* Theta: option price change per 1 passing day */
    /// <summary>
    /// <para>Theta: option price change per 1 passing day.</para>
    /// <para>Get the THETA of an option contract.</para>
    /// <para>S = underlying's price (often denominated as "S0")</para>
    /// <para>K = option's strike price (often denominated as "X")</para>
    /// <para>v = implied volatility</para>
    /// <para>r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")</para>
    /// <para>q = dividend yield (makes for a more precise model)</para>
    /// <para>t = days to expiration (often denominated as "T - t")</para>
    /// <para>NOTE: percentages should be passed as e.g. 0.0310 for 3.10%</para>
    /// </summary>
    public static double Theta(double S, double K, double v, double r, double q, double t, bool isCall, bool isCalendarDays = true, double d1 = 0, double d2 = 0)
    {
        var T = isCalendarDays ? 365 : 252;
        t /= T;

        d1 = d1 != 0 ? d1 : (Log(S / K) + t * (r - q + v * v / 2)) / (v * Sqrt(t));
        d2 = d2 != 0 ? d2 : d1 - v * Sqrt(t);

#if DEBUG
        var call = (1d / T) * (-(((S * v * Exp(-q * t)) / (2 * Sqrt(t))) * Nderivative(d1)) - r * K * Exp(-r * t) * NCD(d2) + q * S * Exp(-q * t) * NCD(d1));
        var put = (1d / T) * (-(((S * v * Exp(-q * t)) / (2 * Sqrt(t))) * Nderivative(d1)) + r * K * Exp(-r * t) * NCD(-d2) + q * S * Exp(-q * t) * NCD(-d1));
#endif
        return isCall ? (1d / T) * (-(((S * v * Exp(-q * t)) / (2 * Sqrt(t))) * Nderivative(d1)) - r * K * Exp(-r * t) * NCD(d2) + q * S * Exp(-q * t) * NCD(d1)) :
                        (1d / T) * (-(((S * v * Exp(-q * t)) / (2 * Sqrt(t))) * Nderivative(d1)) + r * K * Exp(-r * t) * NCD(-d2) + q * S * Exp(-q * t) * NCD(-d1));
    }

    /* Vega: option price change per 1% change in volatility */
    /// <summary>
    /// <para>Vega: option price change per 1% change in volatility.</para>
    /// <para>Get the VEGA of an option contract.</para>
    /// <para>S = underlying's price (often denominated as "S0")</para>
    /// <para>K = option's strike price (often denominated as "X")</para>
    /// <para>v = implied volatility</para>
    /// <para>r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")</para>
    /// <para>q = dividend yield (makes for a more precise model)</para>
    /// <para>t = days to expiration (often denominated as "T - t")</para>
    /// <para>NOTE: percentages should be passed as e.g. 0.0310 for 3.10%</para>
    /// </summary>
    public static double Vega(double S, double K, double v, double r, double q, double t, bool isCall, bool isCalendarDays = true, double d1 = 0, double d2 = 0)
    {
        t /= isCalendarDays ? 365 : 252;

        d1 = d1 != 0 ? d1 : (Log(S / K) + t * (r - q + v * v / 2)) / (v * Sqrt(t));

        return (1d / 100) * S * Exp(-q * t) * Sqrt(t) * Nderivative(d1);
    }

    /* Rho: option price change per 1% change in risk-free rate
       
       generally, calls are more valuable when interest rates are high while
       puts are more valuable when interest rates are low 
    */
    /// <summary>
    /// <para>Rho: option price change per 1% change in risk-free rate.
    /// Generally, calls are more valuable when interest rates are high while
    /// puts are more valuable when interest rates are low.</para>
    /// <para>Get the RHO of an option contract.</para>
    /// <para>S = underlying's price (often denominated as "S0")</para>
    /// <para>K = option's strike price (often denominated as "X")</para>
    /// <para>v = implied volatility</para>
    /// <para>r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")</para>
    /// <para>q = dividend yield (makes for a more precise model)</para>
    /// <para>t = days to expiration (often denominated as "T - t")</para>
    /// <para>NOTE: percentages should be passed as e.g. 0.0310 for 3.10%</para>
    /// </summary>
    public static double Rho(double S, double K, double v, double r, double q, double t, bool isCall, bool isCalendarDays = true, double d1 = 0, double d2 = 0)
    {
        t /= isCalendarDays ? 365 : 252;

        d1 = d1 != 0 ? d1 : (Log(S / K) + t * (r - q + v * v / 2)) / (v * Sqrt(t));
        d2 = d2 != 0 ? d2 : d1 - v * Sqrt(t);

#if DEBUG
        var call = (1d / 100) * K * t * Exp(-(r - q) * t) * NCD(d2);
        var put = -(1d / 100) * K * t * Exp(-(r - q) * t) * NCD(-d2);
#endif

        return isCall ? (1d / 100) * K * t * Exp(-(r - q) * t) * NCD(d2) :
                        -(1d / 100) * K * t * Exp(-(r - q) * t) * NCD(-d2);
    }

    /// <summary>
    /// <para>Get all the Greeks of an option contract.</para>
    /// <para>S = underlying's price (often denominated as "S0")</para>
    /// <para>K = option's strike price (often denominated as "X")</para>
    /// <para>v = implied volatility</para>
    /// <para>r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")</para>
    /// <para>q = dividend yield (makes for a more precise model)</para>
    /// <para>t = days to expiration (often denominated as "T - t")</para>
    /// <para>NOTE: percentages should be passed as e.g. 0.0310 for 3.10%</para>
    /// <para>Example:</para>
    /// <para>var greeks = BlackScholes.Greeks(...); &lt;-- actual calculation</para>
    /// <para>var delta = greeks.delta;</para>
    /// <para>var gamma = greeks.gamma;</para>
    /// </summary>
    public static (double delta, double gamma, double theta, double vega, double rho)
        Greeks(double S, double K, double v, double r, double q, double t, bool isCall, bool isCalendarDays = true, double d1 = 0, double d2 = 0)
    {
        var t1 = t / (isCalendarDays ? 365 : 252);

        d1 = d1 != 0 ? d1 : (Log(S / K) + t1 * (r - q + v * v / 2)) / (v * Sqrt(t1));
        d2 = d2 != 0 ? d2 : d1 - v * Sqrt(t1);

#if DEBUG
        var delta = Delta(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2);
        var gamma = Gamma(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2);
        var theta = Theta(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2);
        var vega = Vega(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2);
        var rho = Rho(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2);
#endif

        return (delta: Delta(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2),
                gamma: Gamma(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2),
                theta: Theta(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2),
                vega: Vega(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2),
                rho: Rho(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2));
    }

    /// <summary>
    /// <para>Get the price and all the Greeks of an option contract.</para>
    /// <para>S = underlying's price (often denominated as "S0")</para>
    /// <para>K = option's strike price (often denominated as "X")</para>
    /// <para>v = implied volatility</para>
    /// <para>r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")</para>
    /// <para>q = dividend yield (makes for a more precise model)</para>
    /// <para>t = days to expiration (often denominated as "T - t")</para>
    /// <para>NOTE: percentages should be passed as e.g. 0.0310 for 3.10%</para>
    /// <para>Example:</para>
    /// <para>var priceGreeks = BlackScholes.PriceGreeks(...); &lt;-- actual calculation</para>
    /// <para>var price = priceGreeks.price;</para>
    /// <para>var delta = priceGreeks.delta;</para>
    /// </summary>
    public static (double price, double delta, double gamma, double theta, double vega, double rho)
        PriceGreeks(double S, double K, double v, double r, double q, double t, bool isCall, bool isCalendarDays = true, double d1 = 0, double d2 = 0)
    {
        var t1 = t / (isCalendarDays ? 365 : 252);

        d1 = d1 != 0 ? d1 : (Log(S / K) + t1 * (r - q + v * v / 2)) / (v * Sqrt(t1));
        d2 = d2 != 0 ? d2 : d1 - v * Sqrt(t1);

#if DEBUG
        var price = Price(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2);
#endif
        var greeks = Greeks(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2);

        return (price: Price(S, K, v, r, q, t, isCall, isCalendarDays, d1, d2),
                delta: greeks.delta, gamma: greeks.gamma, theta: greeks.theta, vega: greeks.vega, rho: greeks.rho);
    }
}
