# DuckDNS Updater
A simple console application that keeps your DDNS Entry on DuckDNS up to date

Run once to generate your config.json, then edit it, providing values provided by [DuckDNS](https://www.duckdns.org)

The default configuration file is as follows:
```
{
  "DoUpdateEveryXMinutes": 5,
  "configfileVersion": 1,
  "sites": [
    {
      "Key": "domain",
      "Value": "token"
    },
    {
      "Key": "domain2",
      "Value": "token2"
    }
  ]
}
```

You can edit it using the json editor of your choice, or by simply filling in the values. If you are only using 1 site, you can remove the second entry, taking care to remove the comma, or add a third entry, making sure to _add_ a comma. IE:

```
  "sites": [
    {
      "Key": "domain",
      "Value": "token"
    }
  ]
```
and 
```
  "sites": [
    {
      "Key": "domain",
      "Value": "token"
    },
    {
      "Key": "domain",
      "Value": "token"
    },
    {
      "Key": "domain",
      "Value": "token"
    }
```

You can have an unlimited number of sites listed. Keep in mind however that if you have for example Hundreds of sites, you may want to increase the time between updates - `"DoUpdateEveryXMinutes": 5,` - so that there is no overlap. It shouldn't cause local issues, other than console spam, but the DuckDNS operators likely wouldn't appreciate it.
