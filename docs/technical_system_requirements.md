# Technical System Requirements

This document covers the technical system requirements and costs for both development and operation of this software.

## Hardware Requirements

This section specifies the minimum hardware required to operate and develop the project. Note: this section applies to both operation *and* development, as the requirements for these are both the same.

| Component | Minimum Requirement |
| --- | --- |
| CPU | Intel Pentium 4 (2004 model) or later |
| RAM | 4 GB |
| GPU | DX10, DX11, and DX12 capable |
| HDD | 8 GB free space |

These requirements are based on the [system requirements for Unity](https://docs.unity3d.com/2022.3/Documentation/Manual/system-requirements.html) and system stress testing.

## Software Requirements

This section specifies the software required to operate or develop the project. Note: this section is split into operation and development, depending on the users needs.

### Operation

The software required to operate this project is listed below:

| Software | Version |
| --- | --- |
| Windows, MacOS or Linux | Windows 10+, Mojave 10.14+ or Ubuntu 18.04+ |
| Unity | 2022.3.13f1 |
| Git | 2.44.0 |

### Development

The minimum software required to develop this project is listed in the [software bill of materials](/docs/software_bom.xlsx), and is listed here for completeness:

| Software | Version |
| --- | --- |
| Windows, MacOS or Linux | Windows 10+, Mojave 10.14+ or Ubuntu 18.04+ |
| Unity | 2022.3.13f |
| Visual Studio Code | 1.87 |
| .NET SDK | 8.0.3 |
| Git | 2.44.0 |

## Costs

This section covers the cost of operation and development of this software. These costs may vary depending on your options.

### Operation

The costs for operating this project include:
- Hardware costs relating to procuring hardware to run the project.
- Software costs relating to license keys for operating systems (if they are not preinstalled on the hardware).

An example platform that could run this software would be the [Acer Aspire 3](https://www.currys.co.uk/products/acer-aspire-3-15.6-laptop-intel-core-i3-128-gb-ssd-silver-10226972.html), costing about £299. Note: this cost may vary and can be cheaper/more expensive depending on the individual users requirements.

Alternatively, a system comprised of low-end processors, a unified storage and terminals could be developed. This would have to be a bespoke implementation for the demonstration at the RAMM. This can be evaluated generously at a cost of ~£150 per terminal (simple processor such as a raspberry pi, and a terminal screen and human basic interface), and a set cost for the unified storage location of £100, due the the low space complexity of our data.

Besides the initial setup, there are no anticipated labour costs relating to maintenance or upkeep of the software. The project is entirely self contained and there are no extra components to maintain (see the [architecture documentation](/docs/architecture/README.md) for more details).

### Development

The costs for developing this project include:
- Hardware costs relating to procuring hardware to run and develop the project.
- Software costs relating to:
    - License keys for operating systems (if they are not preinstalled on the hardware).
    - License keys for the Unity software (if the user chooses not to use the Student or Peronal plans for Unity)

The hardware costs are the same as the costs for operation, and will not be repeated here. The software costs include licenses for the Unity game engine, details of which can be found [here](https://unity.com/pricing). It is recommended that users choose the free plans for Unity (as the software was developed using the Unity Student plan), however if other plans are preferred than the current cost of Unity Pro is €170 per month per seat. It is up to the user to decide what their needs are.
