Welcome to Powker!
===================


**Powker** est un jeu de poker (*Texas Holdem*) en ligne et multi-joueurs, un projet scolaire r�alis� en **C#**.

----------
Index
-------------
[TOC]


Documentation
-------------

Powker est un jeu de poker, de type Texas Holdem, qui ce joue � deux et en ligne.

> **Note:**

> - Client / Serveur
> - Beautiful UI Console Interface
> - Gestion du hasard avanc�e

#### <i class="icon-level-up"></i> Client

Le jeu se jouant � deux, il faudra alors deux clients lanc�s pour pouvoir y jouer. Une fois connect�, chaque client se voit attribuer sa main. Ensuite, le joueur qui n'est pas le dealer d�pose la petite blind et la manche des blinds commence par le dealer. Pour chaque tour, le joueur a le choix parmi les actions suivantes: [**C**]*heck*/[**C**]*all*, [**R**]*aise*, [**F**]*old*, [**A**]*ll-in*

#### <i class="icon-level-down"></i> Serveur

Une fois le serveur lanc�, celui-ci va attendre que deux joueurs se connectent pour pouvoir lancer la partie.



----------


Protocole
-------------------

Le protocole utilise *NetworkComms* pour la communication Client/Serveur ainsi que *protobuf* pour la s�rialisation des donn�es.

> **Note:**

> - Protocole Intelligent.
> - Protocole Rapide.
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

> **Note:** Si vous voulez rajouter des **contexts**, utilisez *protobuf* pour la s�rialisation/d�s�rialisation.

----------
### RandomShuffle

L'algorithme de Shuffle utilis� se r�f�re � celui de la page 32 du livre "*Algorithms*" (4th edition) de Robert Sedgewick [https://algs4.cs.princeton.edu/11model/]:

----------

UML
--------------------
Pour plus d'information [regardez le fichier UML](UML.svg)
![alt text](UML.svg)
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