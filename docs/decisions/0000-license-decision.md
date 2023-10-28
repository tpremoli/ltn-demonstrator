# Choose Licence to Release Project Under

## Context and Problem Statement

Choosing a licence to release our project under is important as it dictates
which software we can use, who can use our project (and how), and how the
project can be distributed. Furthermore, it specifies who the original authors
of the project are and what their responsibilities are to the end users,
including liability, end user support, and release of source code. Finally,
any chosen licence needs to be compatible with the other software components
used in the project.

## Decision Drivers

- Open-source vs proprietary: open-source licences provide transparency and
community collaboration, while proprietary licences offer more control but
limits how others can use the work.
- Compatibility: consideration needs to be made to ensure that software is
compatible with multiple different licences for which there are dependencies.
For this project, the main concern is distributing programs made with Unity -
copyleft/reciprocal licences may not be appropriate as the Unity TOS does not
allow redistribution of source code of the runtime, only the runtime binary
itself. All other components are used either to write code, create assets or
host web applications and do not form part of the actual project and so will
not be distributed as part of the project.
- Distribution and modification: some licences require that derivative works
and distributed versions maintain the same licence (copyleft/reciprocal
licences) while others are more liberal with their requirements for changes
made to the software. This may limit who can use the software as the project
may limit their usage.
- Code dependencies: the chosen licence needs to take into account how the
software interacts with third party software which may be included in the
final product. Again, the main concern is Unity.
- Community and collaboration: the chosen licence will not only impact how
others can use but also how they can modify and make changes to the program.
This may include 
- Software maintenance: licences may include terms which determine how the
author will make changes to the software over time including changes to the
licence itself.
- Commercial usage: licences may specify whether the software may be used to
create revenue for end users.
- Legal implications and liability: licences may also specify what, if any,
liability can be held against the author. Finally, licences may have unintended
legal consequences or may not be enforceable.

## Considered Options

- Public domain
- Proprietary licence
- MIT
- GPL
- Creative Commons

## Decision Outome

Chosen option: MIT, because it is the most flexible, easiest to understand, and
is compatible with other software components which may have proprietary software
licences which mean that components can be redistributed but not source available.
Additionally, being open-source, it encourages collaboration and imposes the fewest
requirements both on the author and the end users. It also reduces the risk of
legal action by limiting the liability of the authors for any damage caused by the
project.

## Consequences

- Good, because MIT is flexible and allows for modification.
- Good, because MIT encourages collaboration by being simple and easy to use.
- Good, because MIT limits author liability and reduces risk of legal action.
- Good, because MIT allows for non-free components to be included in the final
product.
- Bad, because there others could potentially redistribute the work and sell
it for a profit.