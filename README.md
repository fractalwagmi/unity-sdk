# Fractal Unity SDK

<img src="https://i.ibb.co/3dwk9Qg/fractal-unity.png" width="40%"></img>

**Please use the latest build from the release page. Follow the Unity SDK section of [Fractal Developers](https://developers.fractal.is/unity-sdk/introduction) for full integration guide.**

## About

Fractal Unity SDK is designed to get your Unity project up and running with Fractal services in a few easy steps. The package provides all of the Client APIs for interacting with the user's wallet - this includes signing in with Fractal,reading the user's coins and NFTs, and signing on-chain transactions.

## Requirements

Fractal SDK always targets the latest LTS version of **(Currently 2021.3.18f1)**. We recommend you use the LTS version of Unity to build projects that are in production or about to ship. However, you should not encounter any issues when integrating or migrating into any other versions of Unity above the targeted release.

## Supported platforms
- WebGL
- Desktop
- Android 
- iOS

On desktop & mobile builds the default browser displays the authentication prompt. The player session is verified when returned to the game instance.
On WebGL builds the authentication prompt is displayed in a popup without exiting the game session.

## Example

[Open WebGL Demo](https://react-sdk-demo.fractalstaging.com/)

## Changelog

Fractal SDK follows Semantic Versioning `(major.minor.patch)`. Any potential breaking changes will always cause a major version increment, non-breaking new features will cause a minor version increment, and bugfixes will cause a patch version increment.

A full version changelog is available in the [changelog](/CHANGELOG.md) file.


## Installation

1. Download [the latest build from the release page](https://github.com/fractalwagmi/unity-sdk/releases)
2. Import the `.unitypackage` file into your Unity game using the [local asset package import](https://docs.unity3d.com/Manual/AssetPackagesImport.html) process.
3. Optionally select the `Scenes` folder to test out our reference implementation.

If you are developing WebGL title, follow the additional steps below to set up the authentication popup and integration into Fractal "Play now" section.

4. Make sure your projects contain `Plugins` and `WebGL Templates` folder imported from the Fractal SDK package. These
folders include the necessary extensions to handle the web browser's authentication popups.

5. In `Build Settings > Player Settings`, select FractalSDK as your WebGL Template.

## Getting Started

Follow step-by-step instructions and video tutorials in Unity SDK section of [Fractal Developers](https://developers.fractal.is/unity-sdk/introduction)

The integration process is straightforward. First, add authentication prefab to any scene in your project, and then interact with client endpoints as needed using `FractalClient`. SDK requires no 3rd party dependencies, and no additional configuration except entering your project details is necessary.

## Feedback and troubleshooting

- Please submit your feedback using this [form](https://forms.gle/YwhYubuxGGTrYGeaA) after integration.

- If you run into any problems or have a feature request, open up a [new issue](https://github.com/fractalwagmi/unity-sdk/issues/new) in the repository. Please follow the issue/request template.

- Contact us on [Discord](https://discord.gg/fractalwagmi)
