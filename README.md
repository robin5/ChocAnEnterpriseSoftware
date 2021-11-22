# ChocAn Enterprise Software

ChocAn is an organization dedicated to helping people. This repsoitory contains software for managing ChocAn membership and business transactions. The following sections describe the ChocAn enterprise landscape (actors, actions, and expected results).

## Members

Members pay a monthly fee to ChocAn. For this fee they are entitled to unlimited consultations and treatments with health care professionals

## Providers

Dietitians, internists, and exercise experts.

## Membership ID

Every member is given a plastic card embossed with the member’s name and a nine-digit member number and incorporating a magnetic strip on which that information is encoded.

## Provider Terminal

Each health care professional (provider) who provides services to ChocAn members has a specially designed [computer terminal](TERMINAL.md), similar to a credit card device in a shop.

## ChocAn Provider Directory

At any time, a provider can request the software product for a Provider Directory, an alphabetically ordered list of service names and corresponding service codes and fees. The Provider Directory is sent to the provider as an e-mail attachment.

## Main Accounting Procedure

At midnight on Friday, the main accounting procedure is run at the ChocAn Data Center. It reads the week’s file of services provided and prints a number of reports. Each report also can be run individually at the request of a ChocAn manager at any time during the week.

-   [Member Service Report](REPORTS.md)
-   [Provider Service Report](REPORTS.md)
-   [Accounts Payable Summary Report](REPORTS.md)

## Provider and Membership Software

During the day, the software at the ChocAn Data Center is run in interactive mode to allow operators to add new members to ChocAn, to delete members who have resigned, and to update member records. Similarly, provider records are added, deleted, and updated.

## Payment Processing (Acme Accounting)

The processing of payments of ChocAn membership fees has been contracted out to Acme Accounting Services, a third-party organization. Acme is responsible for financial procedures such as recording payments of membership fees, suspending members whose fees are overdue, and reinstating suspended members who have now paid what is owing. The Acme computer updates the relevant ChocAn Data Center computer membership records each evening at 9 P.M.

## Task Boundaries

Your organization has been awarded the contract to write only the ChocAn data processing software; another organization will be responsible for the communications software, for designing the ChocAn provider’s terminal, for the software needed by Acme Accounting Services, and for implementing the EFT component.

## Acceptance Criteria

The contract states that, at the acceptance test:

1. the data from a provider’s terminal must be simulated by keyboard input and
2. the data to be transmitted to a provider’s terminal display must appear on the screen.
3. A manager’s terminal must be simulated by the same keyboard and screen.
4. Each member report must be written to its own file; the name of the file should begin with the member’s name, followed by the date of the report.
5. The provider reports should be handled the same way.
6. The Provider Directory must also be created as a file.
7. None of the files should actually be sent as e-mail attachments.
8. As for the EFT data, all that is required is that a file be set up containing the provider name, provider number, and the amount to be transferred.
