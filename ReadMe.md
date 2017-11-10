Welcome to Powker!
===================


**Powker** est un jeu de poker (*Texas Holdem*) en ligne et multi-joueurs. C'est un projet scolaire réalisé en **C#**.

----------
Index
-------------
[TOC]


Documentation
-------------

Powker est un jeu de poker, de type Texas Holdem, qui ce joue à deux et en ligne.

> **Note:**

> - Client / Serveur
> - Beautiful UI Console Interface
> - Gestion du hasard avancée

#### <i class="icon-level-up"></i> Client

Le jeu se jouant a deux, il faudra alors deux clients lancés pour pouvoir y jouer. Une fois connecté, chaque client se voit attribué sa main. Ensuite le joueur qui n'est pas le dealer dépose la petite blinde et la manche des blindes commence par le dealer. Pour chaque tour, le joueur a le choix parmi les actions suivantes: [**C**]*heck*/[**C**]*all*, [**R**]*aise*, [**F**]*old*, [**A**]*ll-in*

#### <i class="icon-level-down"></i> Serveur

Une fois le serveur lancé, celui-ci va attendre que deux joueurs se connectent pour pouvoir lancer la partie.

#### <i class="icon-pencil"></i> Interface



#### <i class="icon-trash"></i> Abstract



#### <i class="icon-hdd"></i> Class




----------


Protocol
-------------------

Le protocol utilise *NetworkComms* pour la communication Client/Serveur ainsi que *protobuf* pour la sérialisation des données.

> **Note:**

> - Protocol Intelligent.
> - Protocol Rapide.
> - Gestion de la connexion automatique.
> - Modulaire via les contextes.

#### <i class="icon-refresh"></i> Connection

Function                  | Params
--------                  | ---
HandShake                 | `ShortGuid, string, string`
Message                   | `string`

#### <i class="icon-refresh"></i> StartContext

Function                  | Params
--------                  | ---
StartGameContext          | `List<string>, int`
StartHandContext          | `Card , Card , int , int , int , string`
StartRoundContext         | `GameRoundType , List<Card> , int , int`

#### <i class="icon-refresh"></i> EndContext

Function                  | Params
--------                  | ---
EndGameContext            | `string`
EndHandContext            | `Dictionary<string, List<Card>>`
EndRoundContext           | `List<PlayerActionName>`

#### <i class="icon-refresh"></i> TurnContext

Function                  | Params
--------                  | ---
TurnContext               | `int, int, int, int, int, int`


----------

> **Note:** Si vous voulez rajouter des **contexts**, utilisez *protobuf* pour la sérialisation/désérialisation.

----------
### RandomShuffle

L'algorithme de Shuffle utilisé se réfère à celui de la page 32 du livre "*Algorithms*" (4th edition) de Robert Sedgewick [https://algs4.cs.princeton.edu/11model/]:

----------

UML
--------------------

Connection Serveur:

```sequence
Client1->Serveur: HandShake
Note left of Serveur: Serveur Check
Serveur-->Client1: OK
Client2->Serveur: HandShake
Note right of Serveur: Serveur Check
Serveur-->Client2: OK
```

Connection Client:

```flow
s=>start: Start
e=>end: ...
op1=>operation: Input(Sever Address)
cond=>condition: HandShake

s->op1->cond
cond(yes)->e
cond(no)->op1
```


----------


Librairie Externe
--------------------

Network
:   NetworkComms
