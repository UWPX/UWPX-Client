[<img src="https://assets.windowsphone.com/85864462-9c82-451e-9355-a3d5f874397a/English_get-it-from-MS_InvariantCulture_Default.png" width="80">](https://www.microsoft.com/store/apps/9NW16X9JB5WV?ocid=badge)
[![Donate](https://liberapay.com/assets/widgets/donate.svg)](http://liberapay.uwpx.org)
[![Donate](https://www.paypalobjects.com/webstatic/de_DE/i/de-pp-logo-100px.png)](http://paypal.uwpx.org)

| Platform | Branch | Build Status | Latest Version - Store | Latest Version - Sideload |
| :---: | :---: | :---: | :---: | :---: |
| x64 | [master](https://github.com/UWPX/UWPX-Client/tree/master) | [![Build Status](https://dev.azure.com/uwpx/UWPX/_apis/build/status/CI/UWPX%20x64%20CI)](https://dev.azure.com/uwpx/UWPX/_build/latest?definitionId=5) | [v.0.22.0.0](https://www.microsoft.com/store/apps/9NW16X9JB5WV?ocid=badge)  | [v.0.22.0.0](https://github.com/UWPX/UWPX-Client/releases/download/v.0.22.0.0/UWPX_UI_0.22.0.0.zip) |
| x86 | [master](https://github.com/UWPX/UWPX-Client/tree/master) | [![Build Status](https://dev.azure.com/uwpx/UWPX/_apis/build/status/CI/UWPX%20x86%20CI)](https://dev.azure.com/uwpx/UWPX/_build/latest?definitionId=6) | [v.0.22.0.0](https://www.microsoft.com/store/apps/9NW16X9JB5WV?ocid=badge) | [v.0.22.0.0](https://github.com/UWPX/UWPX-Client/releases/download/v.0.22.0.0/UWPX_UI_0.22.0.0.zip) |
| ARM | [master](https://github.com/UWPX/UWPX-Client/tree/master) | [![Build Status](https://dev.azure.com/uwpx/UWPX/_apis/build/status/CI/UWPX%20ARM%20CI)](https://dev.azure.com/uwpx/UWPX/_build/latest?definitionId=7) | [v.0.22.0.0](https://www.microsoft.com/store/apps/9NW16X9JB5WV?ocid=badge) | [v.0.22.0.0](https://github.com/UWPX/UWPX-Client/releases/download/v.0.22.0.0/UWPX_UI_0.22.0.0.zip) |

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/551476163f8f4784bd1017bf2e8f3db1)](https://www.codacy.com/app/COM8/UWPX-Client?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=UWPX/UWPX-Client&amp;utm_campaign=Badge_Grade)

# UWP-XMPP-Client - Now with [OMEMO](https://conversations.im/omemo/) :tropical_fish: support [read on...](https://github.com/UWPX/UWPX-Client/releases)
[This app is still in alpha stage!]

Chat with all your XMPP contacts.

**UWPX is a secure and Open Source XMPP app for all your UWP (Windows 10) devices.**

It implements the E**x**tensible **M**essaging and **P**resence **P**rotocol ([XMPP](https://xmpp.org/)).
At the moment UWPX is still in ALPHA state so expect regular crashes and unexpected behavior.

### Want more up to date news?
Follow [@UWPX_APP](https://twitter.com/UWPX_APP) on ![Twitter](http://i.imgur.com/wWzX9uB.png).

## Table of Contents
1. [Features](#features)
2. [ToDo](#todo)
3. [Alpha/Beta tester](#alphabeta-tester)
4. [Installation](#installation)
5. [Examples](#examples)
6. [References](#references)

## Features:
| Name | XEPs |
| ------------- | ------------- |
| Data Forms | [XEP-0004](https://xmpp.org/extensions/xep-0004.html "XEP-0004") |
| Service Discovery | [XEP-0030](https://xmpp.org/extensions/xep-0030.html "XEP-0030") |
| MUC | [XEP-0045](https://xmpp.org/extensions/xep-0045.html "XEP-0045") |
| Bookmarks | [XEP-0048](https://xmpp.org/extensions/xep-0048.html "XEP-0048") |
| Publish-Subscribe | [XEP-0060](https://xmpp.org/extensions/xep-0060.html "XEP-0060") |
| Chat State | [XEP-0085](https://xmpp.org/extensions/xep-0085.html "XEP-0085") |
| Personal Eventing Protocol | [XEP-0163](https://xmpp.org/extensions/xep-0085.html "XEP-0163") |
| Message Delivery Receipts | [XEP-0184](https://xmpp.org/extensions/xep-0184.html "XEP-0184") |
| Direct MUC Invitations | [XEP-0249](https://xmpp.org/extensions/xep-0249.html "XEP-0249") |
| Message Carbons | [XEP-0280](https://xmpp.org/extensions/xep-0280.html "XEP-0280") |
| Chat Markers | [XEP-0333](https://xmpp.org/extensions/xep-0333.html "XEP-0333") |
| Message Processing Hints | [XEP-0334](https://xmpp.org/extensions/xep-0334.html "XEP-0334") |
| Data Forms - Dynamic Forms | [XEP-0336](https://xmpp.org/extensions/xep-0336.html) |
| Consistent Color Generation | [XEP-0392](https://xmpp.org/extensions/xep-0392.html) |


## ToDo:
| Name | XEPs |
| ------------- | ------------- |
| vcard-temp | [XEP-0054](https://xmpp.org/extensions/xep-0054.html "XEP-0054") |
| User Avatar | [XEP-0084](https://xmpp.org/extensions/xep-0084.html "XEP-0084") |
| Instant Messaging Intelligence Quotient (IM IQ) | [XEP-0148](https://xmpp.org/extensions/xep-0148.html "XEP-0148") |
| Personal Eventing Protocol | [XEP-0163](https://xmpp.org/extensions/xep-0163.html) |
| Jingle | [XEP-0166](https://xmpp.org/extensions/xep-0166.html "XEP-0166") |
|  Jingle Encrypted Transports | [XEP-0391](https://xmpp.org/extensions/xep-0391.html)
| Stream Management | [XEP-0198](https://xmpp.org/extensions/xep-0198.html "XEP-0198") |
| Message Archive Management | [XEP-0313](https://xmpp.org/extensions/xep-0313.html "XEP-0313") |
| HTTP File Upload | [XEP-0363](https://xmpp.org/extensions/xep-0363.html "XEP-0363") |
| Mix | [XEP-0369](https://xmpp.org/extensions/xep-0369.html "XEP-0369") |
| OMEMO (status -> [#5](https://github.com/UWPX/UWPX-Client/issues/5)) | [XEP-0163](https://xmpp.org/extensions/xep-0163.html "XEP-0163") and [XEP-0384](https://xmpp.org/extensions/xep-0384.html "XEP-0384") |

[MORE NOTES TO COME]

## Installation:
There are three ways how you can get access to the app.

### 1. Microsoft Store:
The simplest way is to just download it via the [Microsoft Store](https://www.microsoft.com/store/apps/9NW16X9JB5WV).

[<img src="https://assets.windowsphone.com/85864462-9c82-451e-9355-a3d5f874397a/English_get-it-from-MS_InvariantCulture_Default.png" width="150">](https://www.microsoft.com/store/apps/9NW16X9JB5WV?ocid=badge)

### 2. Sideload:
If you don't want to use the [Microsoft Store](https://www.microsoft.com/store/apps/9NW16X9JB5WV) for getting access to UWPX you can also sideload a pre build app.  
For this head over to [releases](https://github.com/UWPX/UWPX-Client/releases) and download the latest `UWPX_UI_X.Y.Z.0.zip`.  
Once downloaded, unpack and right click `Add-AppDevPackage.ps1` -> `Execute with PowerShell` to install it.

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
<img src="https://user-images.githubusercontent.com/11741404/54202934-67182500-44d1-11e9-8bc6-ce28675aea3f.jpg" width="400"> <img src="https://user-images.githubusercontent.com/11741404/54202932-667f8e80-44d1-11e9-887c-e199c69693ad.jpg" width="400">
<img src="https://user-images.githubusercontent.com/11741404/54202935-67182500-44d1-11e9-8feb-925a1fd49a6c.png" width="400">  

<img src="https://user-images.githubusercontent.com/11741404/54202930-65e6f800-44d1-11e9-92eb-d73e0bbdace1.jpg" width="800">
<img src="https://user-images.githubusercontent.com/11741404/54202931-667f8e80-44d1-11e9-883c-1142fad08763.jpg" width="800">

## References:
This project wouldn’t be possible without the great work of all those people working on the libraries used by UWPX.
[Here](https://uwpx.org/about/) you can find a list of all libraries and other references used for the UWPX development.
So take a second, head over and take a look at their great work!
