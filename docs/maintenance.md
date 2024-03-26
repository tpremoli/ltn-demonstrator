# Maintenance

This document details how to maintain the software. As the project is entirely self contained, there are no external components that need to be maintained or any extra upkeep besides basic system maintenance. However, this guide covers some additional considerations that may be useful concerning this project.

## Updating Environment

The environment on which the project runs should be kept up-to-date to avoid any bugs or security issues. This includes the operating system and any other tools that are used to run or develop the project (depending on the use case). Note: the user should **not** update the Unity software or use a version that is different to the version specified by the [technical system requirements](/docs/technical_system_requirements.md) - this can result in the project being in an unusable state.

## Regular Backups

The project stores some data locally using the JSON format. This data is mostly used to cache certain details of the simulation on disk (e.g. the positions of barriers, journeys for agents to complete, etc.) and used to ensure consistent results across runs of the simulation. The data is generated automatically by the simulation if it does not exist or is missing; with this in mind, it is not absolutely necessary to retain backups of the JSON files as these can be regenerated automatically by the simulation. Nonetheless, it is recommended to keep backups of the saves, which can be found in the file `Assets/Saves/save.json`.

The JSON format is highly flexible and the file size itself is relatively small (less than 300 MB), so the manner in which backups are kept and taken is up to the user. A recommended solution is to copy the file to a secure location with a timestamp at specific time intervals (e.g. once a month). This will ensure that users of the simulation can expect a similar and consistent experience in line with other runs of the simulation.