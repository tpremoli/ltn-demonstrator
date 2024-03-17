# Development Environment

This document explains how to setup a development environment. This can be used to make changes to the software if necessary (e.g. to add/modify/remove certain functionality or to develop additional features, etc.).

## Prerequisites

Before starting, the following prerequisites must be completed:
- The user must have hardware capable of operating and developing the software, see the [technical system requirements](/docs/technical-system-requirements.md) for details. Additionally, the user must have at least one of the operating systems listed in the technical system requirements installed.
- The user must create a Unity account with an appropriate plan for their needs, see the [Unity website](https://unity.com/) for details.
- The user must create a GitHub account and must be granted access to the project GitHub repository.

## Setup

### Visual Studio Code

1. Download and install the latest version of Visual Studio Code from the official website: https://code.visualstudio.com
2. Install the following extensions from the VS Code Marketplace:
    - [Unity](https://marketplace.visualstudio.com/items?itemName=VisualStudioToolsForUnity.vstuc)
    - [GitHub Pull Requests](https://marketplace.visualstudio.com/items?itemName=GitHub.vscode-pull-request-github)

### Git and GitHub

1. Download and install the latest version of Git from the official website: https://git-scm.com/
2. In a terminal, configure Git by setting your username and email address using (note: the email should be the same as the one used for your GitHub account):
    - `git config --global user.name "Your name"`
    - `git config --global user.email "your.email@example.com"`
3. In VS Code, sign in to GitHub using the profile icon.
4. Clone the project repository:
    - From VS Code, bring up the command palette, search for `Git: Clone`. This should bring up a list of options, including `2023-24-UoE-ECMM427/ltn-demonstrator`. Clone this repository to a suitable location.
    - In a terminal, use `git clone https://github.com/2023-24-UoE-ECMM427/ltn-demonstrator` to clone the repository to a suitable location.

### Unity

1. Download and install the latest version of Unity Hub from the official website: https://unity.com
2. Sign in to Unity Hub with your account and set up any licenses.
3. Download and install Unity (specifically, version 2022.3.13f1) from Unity Hub.
4. Open the `ltn-demonstrator` project in Unity Hub.
