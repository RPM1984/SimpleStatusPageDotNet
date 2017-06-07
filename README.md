![](http://i.imgur.com/V1VJJ4O.png)

# SimpleStatusPageDotNet
A simple, low-ceremony way to test the status of your _public_ services, built on ASP.NET Core.

## _Why_ should I use this?
Use this if you want a dead-simple status monitor of your web-based services, and don't want/need all the features of a paid service.

## _How_ can i use this?
- Clone this repository
- Configure your app/health checks (appoptions.json)
- Deploy somewhere. 
- (optional) integrate with a service like Pingdom to hit the root URL of this site, and report to various channels.

## What _does_ this do?
Lets you report on "Websites" or "API's", using basic HTTP pings and health checks.

## What _doesn't_ this do?
Provide historical information on uptime, incident reporting, notifications or maintain any kind of state. If you want any of those features, you're probably better off using [one of these services](https://stackshare.io/status-page-hosting).

# Concepts
In this application, a service is generally considered in one of three states:
- Healthy
- Struggling
- Unhealthy

## Websites
For Websites, a check is performed via a simple _HEAD_ request to a configured URL. A response of 200 (OK), is considered "Healthy", anything else "Unhealthy".

## API's
For API's, a check is performed by hitting a specializied "health" endpoint, with the expectation that the following JSON response schema is returned:
```
{
  "avgResponseTime": {
    "value": {int} // current avg response time (in ms)
  }
}
```

This property allows determining if an API is "Struggling", based on a configured threshold of what is deemed _acceptable_ for a given API. A simple way to return this information could be to keep the last N response times in an in-memory cache, and do a simple _average_ calc).

If you don't need this granularity, stick to "Websites".


# Setup and usage
## Requirements
- Development environment with support for ASP.NET Core 1.1
- Sites/API's open to public (e.g non-secure)

## Styling
- Edit `appoptions.json` with your application name and logo URL
- Edit CSS using `site.css`
- Edit JS using `site.js`

# Remarks
 This application is in no way "perfect". Code could be optimized, HTML/CSS consolidated and new checking techniques configured. But this application was created in less than an hour using `File -> New ASP.NET Core Web Application` and tailored to a simple need.
 
 The hope is that this application will grow over time.
 
 In other words, PR's are very welcome! :)
