## Introduction

In this page, we will guide you with the standards on how to report an issue.

> The standards provided on this page is mostly introduced by the author itself. Please do let us know if you think a change is needed by submitting a pull-request on this page.

## Type of Issues

We considered the following issues.

- [Bug](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reporting%20an%20Issue.md#bug)
- [Request](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reporting%20an%20Issue.md#request)
- [Question](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reporting%20an%20Issue.md#question)
- [Nice-To-Have](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reporting%20an%20Issue.md#nice-to-have)

### Bug

A [bug](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3Abug) is usually prioritized over the other (non-bug) items. If the requestor has enough reason to have the bug be fixed ASAP, the item will be put on top-priority. If the bug is a known-bug, or find not really important (depends on weight), then the bug will not be placed on top-priority list, but it will be picked once the other bugs are done.

> If the bug is top priority, the label [*priority*](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3Apriority) will be set.

**Usual Steps**

Usually, further collaborations are happening between us and the issuer. We will expect from the issuer to give us more insights about the bug.
	
See below, the usual items we asked.

- Basic steps on how to replicate the bug.
- Schema of the table.
- Model or class implementation.
- Log or call stacks.

Please help us speed-up the fix.

**For-Grabs**

Depends on our internal assessment, we sometimes place the bug as an item for [*for-grabs*](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3A%22for+grabs%22) so the community can fix it for us.

Below are some of the considerations for us to place a bug as a *for-grabs* item.

- If somebody has voluteered to fix the bug.
- If we found the bug is not a breaking-change.

**Templates**

We are not requiring a strict template. However, in order to minimize the round-trips of the communication, we would like to ask the issuer to make an effort on the following template.

- ***Title*** - is prefixed with *[Bug]:* or *[Error]:*.
- ***Description*** - must include the following.

	- Name and version of the library (ie: *RepoDb v1.10.4* or *RepoDb.MySql v1.0.3*)
	- Table Schema
	- Class Implemention
	- Call Stack Exception - a screenshot would suffice.
	- Log Files (seldom)

We are thanking you for this!

**Deploying**

Before deploying, if there are existing collaborations (ie: [*GitHub*](https://github.com/mikependon/RepoDb/issues) or [*Gitter*](https://gitter.im/RepoDb/community)), we will contact the issuer first. Otherwise, we will proceed with the deployment.

After the deployment, we will leave the bug open for the next few days (not yet defined) and we will place a [*deployed*](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3Adeployed) label. The label will be placed even the fix is deployed in a pre-release versions.

### Request

A [request](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3Arequest) is an any form of item the issuer would like us to implement (or do). Usually, it is being [assessed](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3A%22under+assessment%22) internally. The weight of the requests to be considered as top priority will be dependent on the community engagement or on our assessment. The higher the engagement ratio, the higher probability that the requests will be prioritized and implemented.

> There will be possibility that the requests will not be implemented if we find it not practical to do so. Sometimes, we placed it in the [*Nice-To-Have*](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reporting%20an%20Issue.md#nice-to-have) items.

It is good to have a good description when requesting an items or features always. Please place a prefix *[Request];* on the title.

We will place the *request* label always.

### Question

A [question](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3Aquestion) is a form of inquiry towards us. The item is open for everyone to answer, no need to wait for the [*for-grabs*](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3A%22for+grabs%22) label. We usually answer to the question ASAP.

A question is not a strict issue, so the issuer can contact us anytime in any forms of communication.

See below.

- [Gitter](https://gitter.im/RepoDb/community)
- [Email](https://repodb.readthedocs.io/en/latest/pages/contact.html)
- [Twitter](https://twitter.com/home) - notifying the [author](https://twitter.com/mike_pendon) or placing a tag *#RepoDb*.

### Nice-To-Have

A [nice-to-have](https://github.com/mikependon/RepoDb/labels/nice%20to%20have) items are list of features or considerations to be done in the future. An item in the nice-to-have may or may-not be done. The weight of items-to-be-done will depend on our own assessment or on the weight of the community collaborations.

> Usually, an engaging and reasonable comments matter in order for us to act on the items.

We are not requiring a strict template for the nice-to-have items. However, we would like to ask the requestor to make an effort on the following template.

- ***Title*** - is prefixed with *[NiceToHave]:* or *[NTH]:*.
- ***Description*** - detailed explanation of the itention or things-to-be-done.

We are thanking you for this!

## Gitter and Twitter

Anyone can contact us via [Gitter](https://gitter.im/RepoDb/community) or [Twitter](https://twitter.com/home) anytime. Usually, the issue reported here is *Question*. However, depends on the outcome of the discussion, we will create an issue on our [*GitHub*](https://github.com/mikependon/RepoDb/issues) page and placing the right type-of-issue.

**We will do the following:**

- Consolidate all the information discussed.
- Create our own *Title* and *Description*.
- Issue a propery label.
- Tag the person whom we have collaborated with.

## StackOverflow

Any one can create a *Question* or *Bug* in [*StackOverflow*](https://stackoverflow.com/questions/tagged/repodb) by tagging RepoDb. We are visiting the site from time-to-time.

**What will we do if there is an issue in StackOverflow?**

- We will answer you directly in StackOverlow.
- We will not report the issue on our GitHub page.

> The questions or bugs issued at *StackOverflow* is not tracked from our history. We recommend that you file an issue directly to our [*GitHub*](https://github.com/mikependon/RepoDb/issues) page.