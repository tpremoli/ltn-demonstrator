# Usage

This document details on how to use the project (e.g. to run the simulation). For details on how to setup a development environment (e.g. to extend or further develop the simulation), see the [development environment guide](/docs/development_environment.md).

## Usage

When opening the simulation, the user is presented with a top down view of the map of Heavitree and a menu which can be used to modify the simulation. A sample of the top down neighbourhood view is given below:

![Neighbourhood View](/docs/images/neighbourhood-view.png)

The neighbourhood consists of several buildings connected by roads and paths, some of which may be restricted by barriers. When the simulation is started, travellers navigate along the routes, taking routes to avoid barriers where necessary.

The main menu of the simulation is given below:

![Main Menu](/docs/images/play-mode-menu.png)

The menu provides several options, including:

- Edit LTN: opens a menu for adding and removing sensors and barriers in the simulation.
- Main Camera: switch to a global camera that can be used to view the entire neighbourhood.
- Cinematic Camera: switch a cinematic camera that views specific areas of the neighbourhood.
- Sensor Cameras: switch to cameras near to user placed sensors.
- More Statistics: open additional statistics that can be used to understand the simulation.
- Simulation Speed: a slider that can be used to increase the speed of the simulation.

Selecting `Edit LTN` brings up the following menu:

![Edit LTN Menu](/docs/images/edit-ltn-menu.png)

This submenu provides several additional options to place barriers and sensors in the simulation. It also contains an option to delete previous saves based on the users preferences. The options are detailed below:

- Play: start the simulation with the given barriers/sensors.
- Add Barriers: allows the user to place a barrier on a road somewhere in the map.
- Add Sensors: allows the user to place a sensor in the simulation.
- Delete Barrier: remove a barrier from the simulation.
- Delete Save: delete a saved instance/setup of the simulation.

Selecting `Add Barriers` brings up the following menu:

![Barrier Menu](/docs/images/barrier-menu.png)

This menu can be used to block certain types of traffic from navigating through the barrier. The types of traffic that can be blocked are listed in the image above.

When the simulation completes, the user is presented with a list of statistics that have been collected during the runtime. These are presented to the user in the statistics screen, and a sample is shown below:

![Statistics Screen](/docs/images/stats-screen.png)

The statistics screen includes a button that can be used to toggle the heatmap of the map, which the user can use to observe which areas of the neighbourhood are worst affected by the current setup of the barriers. A sample of the heatmap is shown below:

![Heatmap](/docs/images/heatmap.png)

Clicking on the button again returns the user to the statistics screen.