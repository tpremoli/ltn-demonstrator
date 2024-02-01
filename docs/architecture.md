# System Architecture
The system created to display the simulation of an LTN uses the Unity game engine in order to do so. It does not communicate with any other services outside of the local machine on which Unity is being run, and as such poses minimal security threats. It also stores no information about the user either locally or remotely. Because of the contained nature of the system, the component diagram is kept relatively simple.

![Image 1]([https://github.com/2023-24-UoE-ECMM427/ltn-demonstrator/blob/CA2/docs/SystemArchitecture.png])



## User: 
The user will be a person interacting with the LTN simulation. This could be developers as they test things, the client, or eventually, members of the public in the Royal Albert Memorial Museum (RAMM). These users will only interact with a device on which the LTN simulation is running, with possibly some help from a more experienced user, such as museum staff/volunteers or a developer.
## Unity: 
The Unity game engine runs the LTN simulation, in which the user is asked to create an LTN by placing barriers on roads, then a simulation is run with these barriers in place. As the simulation runs, some data is collected about the simulation FILL IN THE DATA COLLECTED. None of this data is about the user interacting with the simulation, and it is all stored locally, in the file system as seen in the diagram, to be used for FILL IN THE DATA USAGE.
## Local Machine: 
Unity interacts with the local machine on which it is running by using memory and resources to run the simulation. The local machine contains both Unity and the file system, as the file system is stored locally. 
## File System: 
The file system is stored on the local machine, and includes a list of current ‘saves’, which are configurations of the LTN that the user has saved. These configurations are currently barrier count and positions.

## Other Changes:
The first change is that the information stored in the file system is returned back to the user in the form of some numbers to show the effectiveness of the LTN they designed, it could return things such as total pollution, number of pedestrians etc. These metrics would be similar to those the local council has used in order to test whether the current LTN in Heavitree has been a success.

Note the prototype architecture is slightly different from that of the architecture of the final build. The final build would include multiple instances of the LTN creation, and one master computer. 
The multiple instances of the LTN could run on a portable device, such as an iPad, as provided by the RAMM, where users would create their own LTN, then each instance would send the save (barrier locations) to the master computer, where the master computer would assess which LTN performed the best in terms of some key metrics as mentioned before, such as total pollution, number of pedestrians etc. (THIS SECTION COULD BE EXPANDED OR REMOVED, NEEDS DISCUSSING)

The following are diagrams that could be useful for the rest of the report, or included in this section for completeness (covers multiple pages):

From Ted and Zdenek:



From Oscar and Tomas:


