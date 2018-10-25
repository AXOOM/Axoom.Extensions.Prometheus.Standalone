## 2.0.0
* Make `PrometheusServer` implement `IHostedService` so ASP.NET Core automatically manages its lifetime

## 1.2.0
* Do not pull in hard-coded `PrometheusServer` subsection from config

## 1.1.3
* Do not throw exception on shutdown

## 1.1.2
* Published on GitHub

## 1.1.1 (2018-07-11)

* Show `netsh http add urlacl` hint on startup exception.

## 1.1.0 (2018-07-11)

* Internal refactoring.
* Use default port 5000 if configuration object is passed in but contains no value for `PrometheusServer.Port`.

## 1.0.0 (2018-07-03)

* Initial implementation.
