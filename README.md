# DuckDNS Updater
A simple console application that keeps your DDNS Entry on DuckDNS up to date

Run once to generate your config.json, then edit it, providing values provided by [DuckDNS](https://www.duckdns.org)

The default configuration file is as follows (Messages Removed - They give a brief overview of the config file.):
```
{
  "DoUpdateEveryXMinutes": 5,
  "configfileVersion": 1,
  "sites": [
    {
      "Domain": "domain,domain2",
      "Token": "token",
      "force_ip_number": "",
      "force_ipv6_number": ""
    },
    {
      "Domain": "domain3",
      "Token": "token2",
      "force_ip_number": "",
      "force_ipv6_number": ""
    }
  ]
}
```

You can edit it using the json editor of your choice, or by simply filling in the values. If you are only using 1 site, you can remove the second entry, taking care to remove the comma, or add a third entry, making sure to _add_ a comma. IE:

```
  "sites": [
    {
      "Domain": "example.com",
      "Token": "exmpleToken",
	  "force_ip_number": "",
	  "force_ipv6_number": ""
    }
  ]
```
and 
```
  "sites": [
    {
      "Domain": "example.com",
      "Token": "exmpleToken",
	  "force_ip_number": "",
	  "force_ipv6_number": ""
    },
    {
      "Domain": "example.com",
      "Token": "exmpleToken",
	  "force_ip_number": "",
	  "force_ipv6_number": ""
    },
    {
      "Domain": "example.com",
      "Token": "exmpleToken",
	  "force_ip_number": "",
	  "force_ipv6_number": ""
    }
```

You can update multiple sites that use the same token by listing them as such:
```
"Domain": "example.com,example2.com",
```


You can have an unlimited number of sites listed. Keep in mind however that if you have for example Hundreds of sites, you may want to increase the time between updates - `"DoUpdateEveryXMinutes": 5,` - so that there is no overlap. It shouldn't cause local issues, other than console spam, but the DuckDNS operators likely wouldn't appreciate it.
(A free acount gives you 5 Free sites.)


In theory, this application should work on all platforms where DotNet Core is supported. In Practice, I can not get it to run on my Arch Linux computer, but it does seem to work fine on Debian distributions. I suspect it may have to do with the systems cURL. More investigation to follow.
