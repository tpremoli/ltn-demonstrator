# Development Environment

This document explains how to setup a development environment. This can be used to make changes to the software if necessary (e.g. to add/modify/remove certain functionality or to develop additional features, etc.).

## Prerequisites

Before starting, the following prerequisites must be completed:
- The user must have hardware capable of operating and developing the software, see the [technical system requirements](/docs/technical_system_requirements.md) for details. Additionally, the user must have at least one of the operating systems listed in the technical system requirements installed.
- The user must create a Unity account with an appropriate plan for their needs, see the [Unity website](https://unity.com/) for details.
- The user must create a GitHub account and must be granted access to the project GitHub repository.

## Main Setup

### Setting Up Visual Studio Code

1. Download and install the latest version of Visual Studio Code from the official website: https://code.visualstudio.com
2. Install the following extensions from the VS Code Marketplace:
    - [Unity](https://marketplace.visualstudio.com/items?itemName=VisualStudioToolsForUnity.vstuc)
    - [GitHub Pull Requests](https://marketplace.visualstudio.com/items?itemName=GitHub.vscode-pull-request-github)

### Git Configuration and GitHub Repository Cloning

1. Download and install the latest version of Git from the official website: https://git-scm.com/
2. In a terminal, configure Git by setting your username and email address using (note: the email should be the same as the one used for your GitHub account):
    - `git config --global user.name "Your name"`
    - `git config --global user.email "your.email@example.com"`
3. In VS Code, sign in to GitHub using the profile icon.
4. Clone the project repository:
    - From VS Code, bring up the command palette using ctrl+shift+p (cmd+shift+p), search for the command named `Git: Clone`. This should bring up a list of options, including `2023-24-UoE-ECMM427/ltn-demonstrator`. Clone this repository to a suitable location.
  
### Dealing With Branches

1. With the repo cloned, you should now be in the main branch. You can interact with it using git commands or VSCode. As development happens in branches (To avoid conflicts and keeping the main branch stable), this section will cover that information.
2. List Branches: List all available branches using the following command: 
    - `git branch -v -a`
3. Switch to an existing branch: To switch to an existing branch, type 
    - `git checkout {branch_name}`
4. Where branch_name is the name of the branch without any “remotes/origin/” prefix. Alternatively, in the source control tab of VSCode you can also switch branches: 
5. Your currently active branch can be seen at the bottom left of the screen:
6. REMEMBER: When developing a new, specific feature, place it in a branch! Branches should relatively self-contained, and contain one main overarching feature or bug fix. To then apply your changes to the main branch, we use pull requests, to avoid the main branch from getting clogged up.
7. Create a new branch: To create a new branch from the current branch, use the command 
    - `git checkout -b {branch_name}`
This will create a new branch based on the branch that you’re currently on, meaning that the code will be based on whatever offshoot you’re on. When creating branches for new features, I’d recommend making sure you’re in the main branch before running this command. 
8. Update your local codebase: to make sure that your local branch files are synced with the github repo, use the following command. 
 - `git pull`
Beyond that, it can be helpful to “pull” the latest code from another branch, such as “main” 	to make sure that your git is synced properly before merging 
 - `git pull origin {branch_name}`
9. Merge Branches: To merge your branch with another, say, for example, because a new feature that impacts your work has been added, use the following command: 
    - `git merge {other_branch_name}`
This will essentially pull code from “other_branch_name” and merge it into your currently active branch, and prompt you to resolve any conflicts in the merging that arise. This is particularly useful when trying to merge the branch with the main branch. To ensure you’re following the main branch, it might be useful to use  the command 
    - `git merge main`
10. Make Changes in VSCode: Now that you can mess around with branches, you can make changes to the code in VSCode and  commit the changes to the repo. To do this, make your changes, and save the file. In the source control section, the changes will be shown. You can click on the files to see the changes, and in the left tab, you can add and remove the files to the commit. (with the + and – to the right of the file names): 

Beyond that, in the tab showing the changes, you can select regions of changes and choose to “stage” and “unstage” specific ranges, which will add certain lines to the commit instead of the whole file: 

Once the desired changes have been staged, you can add a commit message and  commit. 

If this is a new branch, once all changes have been committed, you may need to publish it, which will appear in the source control panel. (I already clicked it by accident but you can click it to submit your branch to GitHub): 

11. Look at your branch on GitHub

### Unity

1. Download and install the latest version of Unity Hub from the official website: https://unity.com
2. Sign in to Unity Hub with your account and set up any licenses.
3. Download and install Unity (specifically, version 2022.3.13f1) from Unity Hub.
4. Open the `ltn-demonstrator` project in Unity Hub.

### Working With Unity

1. Now that you have sorted Git branches, you can now open a unity project. Remember where you cloned the repository earlier.
2. The “Unity Project” is located within the ltn-demonstrator directory of the ltn-demonstrator repository. This means that you must open the ltn-demonstrator subfolder shown below:

The “Unity Project” should contain roughly the following files:

3. Now that it’s been opened, any changes you make in VSCode will be recompiled and reflected in the Unity engine. NOTE: Since you can now change branches etc, you can now test different branches and their code. This, as explained later in the document, will allow us to test pull requests, and approve their functionality!
4. As an example, you can try using the previously shown commands to switch to the “buildings-edges-travellers-merge” branch and see how the unity scene changes. You may have to restart the unity engine after switching branch.

### Creating Pull Requests

1. Let’s say you’ve made changes to your branch, and the feature is complete, and ready to be merged with the main branch. First, you must run “git merge main” and fix any conflicts, and then you can create a pull request.
2. Go to the GitHub icon on the left column, and click the little pull request icon:

Pull request icon:

3. Select Branches and write PR: Essentially, you must define the branches to PR through. The default is to merge from your current branch into main (which is pretty much the main use of PRs) but you can change it to whatever your needs are.

Then, you must write a PR containing a descriptive title and body which summarizes the most important changes made, and the rough validation procedure.  Once completed, click “Create”.

### Evaluating Pull Requests

1. Now that you have created a PR, it should appear in GitHub, and in your GitHub tab:
2. To evaluate a pull request, you must first “checkout” the code in it, much like a branch, and test the functionality on your system. This is as simple as right clicking on the PR you want to evaluate, and clicking on “Checkout Pull Request”.
3. This will open the branch in your workspace, for code review, and testing in Unity. Do whatever you need to do here. WARNING: keep an eye on which branch you’re on!  If you make and commit any changes, they will be made to the branch of the pull request, which can be useful for some minor fixes to other’s code, but can get messy! Beyond that, be careful to make sure you only create new branches from the main branch, unless it’s intentional.
4. Once you have reviewed the PR, go on the PR’s site on GitHub, and either request changes if there’s things to be fixed, or approve the changes if everything is validated. Beyond that, if the PR is no longer necessary, it can be closed on GitHub.
5. Once you are done, switch back to either main or whatever branch you were working on! This is important, so you don’t accidentally modify another PR’s code.

### Conclusion
That is the whole setup and workflow for the development environment. Now you should have the capability to use VSCode, Git/GitHub, and Unity, along with dealing with git branches and pull requests. 

Just remember the key points: keep your local code synced with GitHub, stick to guidelines for branches when implementing new features/fixes, and don't forget to give pull requests a thorough check before merging them. This way, you can ensure that everything is handled as smoothly as possible, as you may be working in a large team and have to coordinate between different developers. 


