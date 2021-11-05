[<img src="https://developer.microsoft.com/en-us/store/badges/images/English_get-it-from-MS.png" width="80">](https://www.microsoft.com/store/apps/9N2S5V8HMVLM?ocid=badge)
[![Donate](https://liberapay.com/assets/widgets/donate.svg)](http://liberapay.uwpx.org)
[![Donate](https://www.paypalobjects.com/webstatic/de_DE/i/de-pp-logo-100px.png)](http://paypal.uwpx.org)

| Platform | Branch | Build Status | Latest Version - Store | Latest Version - Sideload |
| :---: | :---: | :---: | :---: | :---: |
| x64 | [master](https://github.com/UWPX/UWPX-Client/tree/master) | [![Build Status](https://dev.azure.com/uwpx/UWPX%20Client/_apis/build/status/CI/Debug%20x64?branchName=master)](https://dev.azure.com/uwpx/UWPX%20Client/_build/latest?definitionId=13&branchName=master) | [v.0.37.0.0](https://www.microsoft.com/store/apps/9NW16X9JB5WV?ocid=badge)  | [v.0.37.0.0](https://github.com/UWPX/UWPX-Client/releases/download/v.0.37.0.0/UWPX-Installer.exe) |
| x86 | [master](https://github.com/UWPX/UWPX-Client/tree/master) | [![Build Status](https://dev.azure.com/uwpx/UWPX%20Client/_apis/build/status/CI/Debug%20x86?branchName=master)](https://dev.azure.com/uwpx/UWPX%20Client/_build/latest?definitionId=15&branchName=master) | [v.0.37.0.0](https://www.microsoft.com/store/apps/9NW16X9JB5WV?ocid=badge) | [v.0.37.0.0](https://github.com/UWPX/UWPX-Client/releases/download/v.0.37.0.0/UWPX-Installer.exe) |
| ARM | [master](https://github.com/UWPX/UWPX-Client/tree/master) | [![Build Status](https://dev.azure.com/uwpx/UWPX%20Client/_apis/build/status/CI/Debug%20ARM?branchName=master)](https://dev.azure.com/uwpx/UWPX%20Client/_build/latest?definitionId=16&branchName=master) | [v.0.37.0.0](https://www.microsoft.com/store/apps/9NW16X9JB5WV?ocid=badge) | [v.0.37.0.0](https://github.com/UWPX/UWPX-Client/releases/download/v.0.37.0.0/UWPX-Installer.exe) |

# UWP-XMPP-Client - Now with Push 📨 support [read on...](https://github.com/UWPX/UWPX-Client/releases/tag/v.0.37.0.0)
[This app is still in beta stage!]

Chat with all your XMPP contacts.

**UWPX is a secure and Open Source XMPP app for all your UWP (Windows 10 and 11) devices.**

It implements the E**x**tensible **M**essaging and **P**resence **P**rotocol ([XMPP](https://xmpp.org/)).
At the moment UWPX is in BETA stage so expect unexpected behavior sometimes.

### Want more up to date news?
Follow [@UWPX_APP](https://twitter.com/UWPX_APP) on ![Twitter](http://i.imgur.com/wWzX9uB.png).

## Table of Contents
1. [Features](#features)
2. [ToDo](#todo)
3. [Installation](#installation)
4. [Examples](#examples)
5. [References](#references)

## Features:
| Name | XEPs | Version |
| ------------- | ------------- | ------------- |
| Data Forms | [XEP-0004](https://xmpp.org/extensions/xep-0004.html "XEP-0004") | 2.10.0 (2020-05-05) |
| Service Discovery | [XEP-0030](https://xmpp.org/extensions/xep-0030.html "XEP-0030") | 2.5rc3 (2017-10-03) |
| MUC | [XEP-0045](https://xmpp.org/extensions/xep-0045.html "XEP-0045") | 1.32.0 (2019-05-15) |
| Bookmarks | [XEP-0048](https://xmpp.org/extensions/xep-0048.html "XEP-0048") | 1.32.0 (2019-05-15) |
| Publish-Subscribe | [XEP-0060](https://xmpp.org/extensions/xep-0060.html "XEP-0060") | 1.18.0 (2020-02-27) |
| Chat State | [XEP-0085](https://xmpp.org/extensions/xep-0085.html "XEP-0085") | 2.1 (2009-09-23) |
| Personal Eventing Protocol | [XEP-0163](https://xmpp.org/extensions/xep-0163.html "XEP-0163") | 1.2.1 (2018-03-18) |
| Message Delivery Receipts | [XEP-0184](https://xmpp.org/extensions/xep-0184.html "XEP-0184") | 1.4.0 (2018-08-02) |
| XMPP Ping | [XEP-0199](https://xmpp.org/extensions/xep-0199.html "XEP-0199") | 2.0.1 (2019-03-26) |
| Direct MUC Invitations | [XEP-0249](https://xmpp.org/extensions/xep-0249.html "XEP-0249") | 1.2 (2011-09-22) |
| Message Carbons | [XEP-0280](https://xmpp.org/extensions/xep-0280.html "XEP-0280") | 0.13.2 (2019-12-16) |
| Message Archive Management **(EXPERIMENTAL)** | [XEP-0313](https://xmpp.org/extensions/xep-0313.html "XEP-0313") | 0.7.2 (2020-09-29) |
| Chat Markers | [XEP-0333](https://xmpp.org/extensions/xep-0333.html "XEP-0333") | 0.4 (2020-04-15) |
| Message Processing Hints | [XEP-0334](https://xmpp.org/extensions/xep-0334.html "XEP-0334") | 0.3.0 (2018-01-25) |
| Data Forms - Dynamic Forms | [XEP-0336](https://xmpp.org/extensions/xep-0336.html) | 0.2 (2015-11-09) |
| Push Notifications | [XEP-0357](https://xmpp.org/extensions/xep-0357.html) | 0.4.1 (2020-02-11) |
| Unique and Stable Stanza IDs | [XEP-0359](https://xmpp.org/extensions/xep-0359.html) | 0.6.0 (2018-10-01) |
| HTTP File Upload **(DOWNLOAD)** | [XEP-0363](https://xmpp.org/extensions/xep-0363.html "XEP-0363") | 1.0.0 (2020-02-11) |
| OMEMO [#5](https://github.com/UWPX/UWPX-Client/issues/5), [#125](https://github.com/UWPX/UWPX-Client/issues/125) **(EXPERIMENTAL)** | [XEP-0384](https://xmpp.org/extensions/xep-0384.html "XEP-0384") | 0.8.1 (2021-10-07) |
| Consistent Color Generation | [XEP-0392](https://xmpp.org/extensions/xep-0392.html) | 0.7.0 (2019-10-16) |
| Stanza Content Encryption | [XEP-0420](https://xmpp.org/extensions/xep-0420.html) | 0.4.0 (2021-04-07) |


## ToDo:
| Name | XEPs | Version |
| ------------- | ------------- | ------------- |
| vcard-temp | [XEP-0054](https://xmpp.org/extensions/xep-0054.html "XEP-0054") | 1.2 (2008-07-16) |
| User Avatar [#130](https://github.com/UWPX/UWPX-Client/issues/130) | [XEP-0084](https://xmpp.org/extensions/xep-0084.html "XEP-0084") | 1.1.4 (2019-09-20) |
| Instant Messaging Intelligence Quotient (IM IQ) | [XEP-0148](https://xmpp.org/extensions/xep-0148.html "XEP-0148") | 1.0 (2005-04-01) |
| Personal Eventing Protocol | [XEP-0163](https://xmpp.org/extensions/xep-0163.html) | 1.2.1 (2018-03-18) |
| Jingle | [XEP-0166](https://xmpp.org/extensions/xep-0166.html "XEP-0166") | 1.1.2 (2018-09-19) |
| Jingle Encrypted Transports | [XEP-0391](https://xmpp.org/extensions/xep-0391.html) | 0.1.2 (2018-07-31) |
| Stream Management | [XEP-0198](https://xmpp.org/extensions/xep-0198.html "XEP-0198") | 1.6 (2018-07-25) |
| HTTP File Upload **(UPLOAD)** | [XEP-0363](https://xmpp.org/extensions/xep-0363.html "XEP-0363") | 1.0.0 (2020-02-11) |
| Mix | [XEP-0369](https://xmpp.org/extensions/xep-0369.html "XEP-0369") | 0.14.4 (2020-03-26) |

[MORE NOTES TO COME]

## Installation:
There are three ways how you can get access to the app.

### 1. Microsoft Store:
The simplest way is to just download it via the [Microsoft Store](https://www.microsoft.com/store/apps/9NW16X9JB5WV).

[<img src="https://developer.microsoft.com/en-us/store/badges/images/English_get-it-from-MS.png" width="150">](https://www.microsoft.com/store/apps/9N2S5V8HMVLM?ocid=badge)

### 2. Sideload:
If you don't want to use the [Microsoft Store](https://www.microsoft.com/store/apps/9NW16X9JB5WV) for getting access to UWPX you can also sideload a pre build app.  
For this head over to [releases](https://github.com/UWPX/UWPX-Client/releases) and download the latest `UWPX-Installer.exe`.  
Once downloaded, run it and you are ready to go!

**For this to work you first have to enable `Sideload app` in your Windows settings! Click [here](https://www.windowscentral.com/how-enable-windows-10-sideload-apps-outside-store) for more information about this.**

### 3. Build it by your own
#### Short version:
1. Install [Visual Studio 2019](https://www.visualstudio.com/de/downloads)
2. Clone the repo with Visual Studio 2019
3. Build the project for your target platform (e.g. `x64`)
4. Install the app on your target system:
[Here](https://docs.microsoft.com/en-us/windows/uwp/get-started/enable-your-device-for-development) you can find more information about: How to install UWP apps, using the developer mode.

#### Long version:
An extended guide on how to build UWP with images to guide you through can be found [here](https://uwpx.org/development/).

## Examples:

### Chat
<img src="https://user-images.githubusercontent.com/11741404/132220103-2b4767e4-edff-418e-b97e-5b53b2e1ad11.png" width="800" height="auto"/>
<img src="https://user-images.githubusercontent.com/11741404/132220142-ec03de28-089a-4fc8-b9a9-33ea6aca7421.png" width="800" height="auto"/>

### Group Chat
<img src="https://user-images.githubusercontent.com/11741404/132220121-c1c54f2c-a712-4089-889b-5968ea505c48.png" width="800" height="auto"/>
<img src="https://user-images.githubusercontent.com/11741404/132220150-ff1c50cf-65e0-4311-837a-5c8c978dc43d.png" width="800" height="auto"/>

### Contacts and OMEMO
<img src="https://user-images.githubusercontent.com/11741404/132220115-ea13e993-8da5-4708-8f32-5e558b9a7bb1.png" width="800" height="auto"/>
<img src="https://user-images.githubusercontent.com/11741404/132220145-55ae61e1-ea47-4351-be78-bac41481347d.png" width="800" height="auto"/>

### Registration
<img src="https://user-images.githubusercontent.com/11741404/132220129-8e32c7f8-7d88-4a19-b89e-4cf6e2696875.png" width="800" height="auto"/>
<img src="https://user-images.githubusercontent.com/11741404/132220166-325b0991-3c93-4020-ad3a-a4f906ba0f47.png" width="800" height="auto"/>

## References:
This project wouldn’t be possible without the great work of all those people working on the libraries used by UWPX.
[Here](https://uwpx.org/about/) you can find a list of all libraries and other references used for the UWPX development.
So take a second, head over and take a look at their great work!
