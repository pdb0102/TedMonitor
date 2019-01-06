# TedMonitor
[The Energy Detective (TED)](https://theenergydetective.com/) Monitoring Application to automatically generate Magic Mirror module data

.Net application implementing a C# API and timer process to retrieve data from a TED ECC and to generate HTML fragments that can be used by the [MMM-TheEnergyDetective](https://github.com/pdb0102/MMM-TheEnergyDetective) module.

````
Usage: TedMonitor /h=<hostname> [/i=<refresh-interval>] [/o=<out-dir>]
                  [/f=<fade-start-%>] [/s=<max_solar_kw>]

/h=<hostname>     The hostname or IP address of your TED ECC
/i=<refresh-int>  The interval, in seconds, to wait between refreshing TED data
                  and updating output files
/o=<out-dir>      The directory the output files are written to
/f=<fade-start>   If specified, the percentage at which spyder list should
                  fade to black. If not specified, no fade is applied.
/s=<max_solar_w>  The maximum output, in watts, of the solar/generation system,
                  if a Generation MTU is exists in the system. Required to
                  generate the 'solar-now' output
/d                Enable debug output


Generated output files:
spyder-htol.ted:
        Lists 'Present' and 'Today' values for all spyder groups, sorted from
        highest to lowest 'present' value. Ignores any group
        with no data for 'Today'

spyder-atoz.ted:
        Lists 'Present' and 'Today' values for all spyder groups, sorted
        alphabetically  by group name. Ignores any group with no
        data for 'Today'

solar-now.ted:
        Generates a CSS dial showing the current output of the 'Generation MTU'.
        Requires /s= parameter.

dashboard.ted:
        Generates 'Energy Overview' of all MTUs in the system.

summary.ted:
        Generates 'Energy Summary' showing 'Present', 'Today', 'This Month' and
        'Projected' kWh values from the ECC
````

The code is a bit messy - I pulled the parts I needed from my home management app.