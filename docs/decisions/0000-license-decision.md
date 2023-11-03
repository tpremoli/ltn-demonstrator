# Choose a Licence to Release Project Under

## Context and Problem Statement

Choosing a licence to release our project under is important as it dictates
which software we can use, who can use our project (and how), and how the
project can be distributed. Furthermore, it specifies who the original authors
of the project are and what their responsibilities are to the end users,
including liability, end user support, and release of source code. Finally, any
chosen licence needs to be compatible with the other software components used
in the project.

## Decision Drivers

- Open-source vs proprietary: open-source licences provide transparency and
  community collaboration, while proprietary licences offer more control but
  limits how others can use the work. Additionally, open-source licences are
  typically used for scientific research or projects where it is expected that
  others may want to tinker and experiment with the software, while proprietary
  licences are used for industrial applications where it is expected that
  the user will only use the application.
- Compatibility: consideration needs to be made to ensure that software is
  compatible with multiple different licences for which there are dependencies.
  For this project, the main concern is distributing programs made with Unity -
  copyleft/reciprocal licences may not be appropriate as the Unity TOS does not
  allow redistribution of source code of the runtime, only the runtime binary
  itself. The only other component used as part of the software is the .NET
  runtime Mono (which is used by Unity), which is also licensed under the MIT
  licence and so is compatible with most if not all other open source software
  licences. All other components are used either to write code, create assets
  or host web applications and do not form part of the actual project and so
  will not be distributed as part of the project - they are tools used to
  develop the project and not actual software components used as part of the
  final product.
- Distribution and modification: some licences require that derivative works
  and distributed versions maintain the same licence (copyleft/reciprocal
  licences) while others are more liberal with their requirements for changes
  made to the software. This may limit who can use the software as the project
  may limit their usage.
- Code dependencies: the chosen licence needs to take into account how the
  software interacts with third party software which may be included in the
  final product. Again, the main concern is Unity.
- Community and collaboration: the chosen licence will not only impact how
  others can use but also how they can modify and make changes to the program,
  or even potentially redistribute the program to further users. These are both
  important to consider as the software may not be directly marketed/used by
  us, instead it may be used by users such as the council who may wish to
  provide it to third parties.
- Software maintenance: licences may include terms which determine how the
  author will make changes to the software over time including changes to the
  licence itself. While efforts will be made to ensure that the software is
  maintained appropriately, these can either be ratified or disclaimed using a
  licence.
- Commercial usage: licences may specify whether the software may be used to
  create revenue for end users. Alternatively, it may specify any royalties or
  other fees that may be payable to the authors, distributors, etc. in order to
  use/sell the software onwards.
- Legal implications and liability: licences may also specify what, if any,
  liability can be held against the author. Finally, licences may have
  unintended legal consequences or may not be enforceable.

## Considered Options

- Public domain
- Proprietary licence
- MIT
- GPL
- Creative Commons
- Apache

## Decision Outcome

Chosen option: MIT, because it is the most flexible, easiest to understand, and
is compatible with other software components which may have proprietary
software licences which mean that components can be redistributed but not
source available. Additionally, being open-source, it encourages collaboration
and imposes the fewest requirements both on the author and the end users. It
also reduces the risk of legal action by limiting the liability of the authors
for any damage caused by the project. Overall, the cost of this licence is
minimal (both to us as developers and to any third parties who may be
interested in using, redistributing or modifying the code) while the benefits
are significant when compared with other licences such as GPL, Creative Commons
or using a custom proprietary licence.

## Consequences

- Good, because using the MIT licence ensures the software is accessible,
  modifiable and redistributable by a wide audience (provided they include
  proper attribution to us as the authors).
- Good, because MIT is flexible and allows for modification.
- Good, because MIT encourages collaboration by being simple and easy to use.
- Good, because MIT limits author liability and reduces risk of legal action.
- Good, because MIT allows for non-free components to be included in the final
  product without any issues.
- Bad, because there others could potentially redistribute the work and sell it
  for a profit.

## Options

### Public domain

By placing the IP in the public domain, the team will relinquish all ownership
and copyright rights. Others can modify, use and distribute the product without
restriction. This option may not be possible under all legal jurisdictions.

- Good, because this has the least cost out of all the licences.
- Bad, because this option is not a real licence so there are no real
  rules/stipulations regarding usage, modification, redistribution or
  attribution.

For our purposes, this choice was not suitable as it could lead to potential
adverse impact to either us as the project owners or the clients.

### Proprietary licence

This options allows the team to retain control over how our software is used
and distributed. It will not be accessible without a purchased licence.

- Good, because it enables the team to maintain full control over the code.
- Bad, as either the cost is high (implementation would require potentially
  meeting with a solicitor) or the potential adverse impact is high (a
  "homebrew" licence may not be completely reliable).
- Bad, because proprietary licences are typically more adverse to
  external/third party development and may lack the tools to support outside
  interest.

For our purposes, this choice was not suitable as it was deemed to costly to
carry out effectively and would not be suitable for the purposes of fostering
collaboration and community efforts into this project.

### MIT licence

MIT is a permissive open-source license that allows users to do almost
anything with our code as long as they include our original copyright notice.
This is unlike copyleft licences such as GPL, which require that the licence be
included in all or most parts of the software, and also stipulate that
libraries included in the software are also compatible with the original
licence.

This licence was chosen as it was found to be the most suited to our needs when
compared with the other options.

### GPL

The GPL is a copyleft license that requires any modified versions of our
software to also be open-source and released under the same license. The GNU
Lesser General Public License (LGPL) is a more permissive variation of the GPL.

- Good, because it encourages external development and outside interest.
- Good, because it is a mature licence which is widely used in industry.
- Bad, because it requires that libraries included in the project are also
  non-free.
- Bad, because it requires that the licence is included with all copies of the
  software include the licence.

For our purposes, this option was deemed infeasible as the Unity runtime/engine
is non-free, and so would not comply with this licence.

### Creative Commons

These licenses are often used for non-software works, such as creative content,
but can be adapted for software. They provide various levels of permissions and
restrictions.

- Good, because there is a large degree of flexibility available with these
  series of licences enabling both us, the clients and the end users to achieve
  our needs.
- Bad, as these licences are typically used for creative works and not
  software, and so may not include appropriate clauses regarding, e.g.
  modification or redistribution.

For our purposes, this choice was found to be inferior to the chosen choice as
the MIT licence is more appropriate for software.

### Apache Licence

Similar to the MIT licence, the Apache licence is a free, permissive licence
which does not contain clauses similar to those found in copyleft licences such
as GPL requiring derivative works to also be licensed under GPL. It is also
designed to be compatible with GPL.

- Good, because it is compatible with other software used in this project.
- Good, because it encourages external development and research.
- Bad, as the licence is significantly longer compared with MIT and is
  typically implemented by including the licence in all source code files.

For our purposes, this licence was found to be inferior compared to MIT as the
licence is significantly longer and more difficult to implement.
