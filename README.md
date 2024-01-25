# Software Demonstrator for LTNs for RAMM

## Executive Summary

In our increasingly urbanised society, the organisation of neighbourhood design becomes increasingly important. The layouts and permitted uses of our spaces can influence our daily lives in a number of different ways, such as where and how we socialise, our navigation tendencies, along with quality of life indicators such as pollution concentration and the noise levels.

LTNs (Low Traffic Neighbourhoods), designed to reduce traffic, involves blocking off vehicular access on certain roads, and to prioritise pedestrians and cyclists. Another goal of these neighbourhoods is to improve quality of life indicators; increasing the resident's daily exercise, and diverting heavy traffic to arterial roads. 

There is currently a heated debate on LTNs in Exeter, specifically on a trial being implemented in the Heavitree and Whipton Neighbourhoods [1]. Residents of these neighbourhoods and workers at the Exeter council are largely impacted by this debate, however, we believe that there is a gap in knowledge and understanding of how these sorts of neighbourhoods work. As a result of this, we seek to implement an intuitive, interactive traffic simulation of LTNs. 

This program will simulate a road layout environment in which users interact, with permissions such as placing barriers on different streets in the neighbourhood. Upon execution, the simulation should record relevant metrics which will be presented through the user interface, in graphical form. Changes in metrics on the environment can be tracked by the user serving as an education tool regarding the holistic implications of LTNs.

The goals of the simulation are primarily, as an education tool to serve to inform them about LTNs and their impact on a community. Secondly, to enhance decision-making regarding implementation of LTNs, while promoting transparency and understanding of people's needs. Finally, to reduce conflicts in the debate arising from misinformation, or the lack of information.

By presenting data in an engaging and digestible format, we educate users, helping them make more informed decisions based on their improved understanding of LTN dynamics. Users should gain a clearer, less biased understanding on how LTNs function, as well as the impacts that they have on micro and on macro scales. We will enable users to visualise the tangible benefits and potential challenges of implementing LTNs, along with providing a degree of transparency to the operation of LTNs.

Whilst designing the project, numerous risks have been identified. We will ensure that the limitations of the simulation are clear, and that this tool is not primarily intended as a tool used to design LTNs. Our tool is an educational tool, and not meant to be used as an exact simulation (without modification and verification). More risks are explored in the risk management section.

Our proposed simulation seeks to demystify the complexities of neighbourhood design, and increase the understanding we have about LTNs as a whole. By providing an interactive and informative tool, we aim to foster more constructive dialogue and decisions surrounding this crucial urban development topic.


[1]: https://www.devon.gov.uk/news/heavitree-and-whipton-active-streets-trial-begins-today/

## Technical System Requirements

The following two subsections outlines the system requirement necessary for running and development respectively. Note that these figures are subject to change.

### Deployement

The application is fully self-contained to the end-user and does not require any server or other resources beyond a machine to be ran on.

The Unity software supports export to a variety of platforms, and you may see their system requirements in [2]. At the moment our implementation of the LTN Simulator supports desktop only. The system requirements can be seen [here](https://docs.unity3d.com/Manual/system-requirements.html#desktop).

An example system that fulfills those criteria is Raspberry Pi 5, priced at [60 GBP](https://thepihut.com/products/raspberry-pi-5?src=raspberrypi&variant=42531604922563). Note that this does not include power cable, (priced at betwen [6](https://uk.rs-online.com/web/p/raspberry-pi-power-supplies/1873417?cm_mmc=UK-PLA-DS3A-_-google-_-CSS_UK_EN_PMAX_Catch+All-_--_-1873417&matchtype=&&gad_source=1&gclid=Cj0KCQiAqsitBhDlARIsAGMR1Ri02gqkj9Kgj6hY_vOYhE8HqKdiJeC1602vF6URd893YvAXE_LMzXwaAuE4EALw_wcB&gclsrc=aw.ds) to [12](https://cpc.farnell.com/raspberry-pi/sc1152/rpi-5-27w-usb-c-psu-eu-white/dp/SC20195?mckv=s_dc|pcrid|605262956803|kword||match||plid||slid||product|SC20195|pgrid|138313687415|ptaid|pla-1678231542173|&CMP=KNC-GUK-CPC-SHOPPING-9262013734-138313687415-SC20195&s_kwcid=AL!5616!3!605262956803!!!network}!1678231542173!&gclid=Cj0KCQiAqsitBhDlARIsAGMR1RgfKnCaeAR100Jw6ifS97C_uY2R5PMc8Gdt1EpFfWhmt51tNO1WBigaAj58EALw_wcB) GBP, the power requirements for the application are unknown at this time,) and other periferies, such as mouse, keyboard and computer screen.

In other words, most generic purpose home computers are suitable to run this program. The exact requirements on the computing capacity of the computer are going to be known later in the development.

No maintenance is expected.

[2]: https://docs.unity3d.com/Manual/system-requirements.html

### Development

Continued development requires a licence to the Unity software provided by Unity Technologies, which can be obtained [here](https://unity.com/pricing). Note that depending on the client's needs, either a personal (free) or Pro version (priced at 170 EUR per developer per month) license can be obtained. Note that any potential developer should consult legal advice, (price not estimated) as to which licence is suitable for them.

Further development is also going to require access to a machine, per developer. Unity Editor [system requirements](https://unity.com/download) lists:

> **OS**:
> Windows 7 SP1+, 8, 10, 64-bit versions only; Mac OS X 10.13+; Ubuntu 16.04, 18.04, and CentOS 7.
> 
> **GPU**: 
> Graphics card with DX10 (shader model 4.0) capabilities.

Most machines should be suitable.

Note that it may be necessary to write additional code, in which case a separate IDE license may be required.