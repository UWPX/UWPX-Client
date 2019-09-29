# Privacy Policy:

## General:
UWPX is an XMPP client, that connects to XMPP servers.
It does not collect any personal data and all ways data is collected can be disabled.

## Crash reporting:
After a crash, the App will collect data about what happened and uploads this bundle to [App Center](https://appcenter.ms) or for versions lower than [v.0.6.0.0](https://github.com/UWPX/UWPX-Client/releases/tag/v.0.6.0.0) to [HockeyApp](https://hockeyapp.net/features/crashreports/).<br>
Althoug it's not recommended, it can be disabled via `Settings` -> `Misc` -> `Disable crash reporting`.

<details>
<summary>Example report:</summary>
<pre>
Incident Identifier: 9f87a925-2d28-40d1-9612-02b3c8cfc1d7
CrashReporter Key:   t7vgZ+qEyZITMCMsMVzbTvb7V0n6zB7UmdlztKvfoBk=
Hardware Model:      Z270-HD3P
Identifier:      UWP_XMPP_Client
Version:         0.1.0.0

Date/Time:       2017-12-24T11:46:21.022Z
OS Version:      Windows 10.0.16299.125
Report Version:  104

Exception Type:  System.AggregateException
Crashed Thread:  2

Application Specific Information:
A Task's exception(s) were not observed either by Waiting on the Task or accessing its Exception property. As a result, the unobserved exception was rethrown by the finalizer thread. (Object reference not set to an instance of an object.)

Exception Stack:
unknown location
Data_Manager2.Classes.DBManager.ImageManager.<>c__DisplayClass9_0.<<contiuneAllDownloads>b__0>d.MoveNext()
</pre>
</details>

## Analytics:
Since version [`0.11.0.0`](https://github.com/UWPX/UWPX-Client/releases/tag/v.0.11.0.0) UWPX uses [AppCenter](https://appcenter.ms) [Analytics](https://docs.microsoft.com/en-us/appcenter/analytics/) to report basic information like session count, duration and which OS version you are running on.
Like Crash Reporting, Analytics can be disabled via `Settings` -> `Misc` -> `Disable analytics`.

## Accounts:
XMPP accounts you add, get stored in a local [SQLite-net database](https://github.com/praeclarum/sqlite-net). This database is located in the apps [ApplicationData](https://docs.microsoft.com/en-us/uwp/api/windows.storage.applicationdata) folder, where by design no other applications have access to.<br>
The password for each account gets stored in a [PasswordVault](https://docs.microsoft.com/en-us/uwp/api/windows.security.credentials.passwordvault) ([implementation](https://github.com/UWPX/UWPX-Client/blob/master/Data_Manager2/Classes/Vault.cs)).<br>
Passwords for [MUC](https://xmpp.org/extensions/xep-0045.html)s and all [OMEMO](https://xmpp.org/extensions/xep-0384.html) keys get stored in plain text in the above mentioned database.

## Picture library:
The App accesses your picture library to store downloaded images.<br>
This can be disabled via `Settings` -> `Chat` -> `Store images in library`.<br>
If disabled, the app will store all newly downloaded images in the local app directory.

## Bluetooth
Since version [`0.20.0.0`](https://github.com/UWPX/UWPX-Client/releases/tag/v.0.20.0.0) UWPX uses Bluetooth to connect and register new experimental XMPP IoT devices.

## Webcam
Since version [`0.20.0.0`](https://github.com/UWPX/UWPX-Client/releases/tag/v.0.20.0.0) UWPX can use your webcam to scan XMPP realted Qr-Codes. This is used for scanning/validating OMEMO fingerprints and experimental XMPP IoT setup QR-Codes.
