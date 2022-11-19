# **Black-Scholes-Merton Asset Pricing Model**
**Library: .NET implementation of the Black-Scholes-Merton asset pricing model.**<br><br>

**Lightweight** and **fast .NET implementation** of one of the most commonly used pricing models in finance.<br><br>
This model comes from Black-Scholes' "The Pricing of Options and Corporate Liabilities" (1973) revised by Merton the same year in "Theory of Rational Option Pricing" to account for dividends and is the most used to this day.<br><br>

**Example of put option:**<br>
<img src="https://user-images.githubusercontent.com/96583994/202844390-f89311b9-87e7-414f-a89f-e17d03d1e0cb.png"><br><br>

-------------------------------------
**Inputs explained**<br>
            S = underlying price (often denominated as "S0")<br>
            K = option's strike price (often denominated as "X")<br>
            v = implied volatility<br>
            r = risk-free rate (is advised the "U.S. 10 Year Treasury Rate")<br>
            q = dividend yield (makes for a more precise model)<br>
            t = days to expiration (often denominated as "T - t")<br>

-------------------------------------
**What to expect**<br>
- The option pricing model is an **estimator of approximate fair value**, it does **NOT** predict the future nor it pretends to, so especially on numbers <0.01 or assets with low volume expect a discrepancy from actual market data.
The model works as intended.

-------------------------------------
**Limitations of the Black-Scholes-Merton model**<br>
Assumtions are made in the model that make the predicted value only valid at the time of estimation, if hypothetically these assumptions held true during the lifetime of the contract then the price prediction would be deterministic.<br>
It is assumed that:<br>
- costs for option transfer is not accounted for (so assumed to be 0)
- the risk-free rate and volatility of the underlying asset are known and do not change
- the returns of the underlying asset can be explained by a normal distribution
- the option is European (i.e. can only be exercised at expiration)

A detailed explaination of the model can be found [here](https://www.macroption.com/black-scholes-formula), an implementation example is [here](https://brilliant.org/wiki/black-scholes-merton/#high-level-explanation-of-the-black-scholes-merton-formula).

-------------------------------------
**NOTE**<br>
Legally this comes with no warranty and you are using it at your own risk.<br><br>
This library have been tested agaist real market data and its results hold correct.<br>
If you find an issue with the results or implementation or an optimization could be made please feel free to contact me or issue a pull request.<br>
You will be credited for any contribution.<br><br>
Remember to have fun.

