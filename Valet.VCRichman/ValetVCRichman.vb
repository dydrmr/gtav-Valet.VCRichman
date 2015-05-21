Imports GTA
Imports GTA.Math
Imports System
Imports System.IO
Imports System.Text
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Threading.Tasks
Imports System.Collections.Generic

Public Class ValetVCRichman
    Inherits Script

    Public isMinigameActive As Boolean = False
    Public Shared CurrentMinigameStage As MiniGameStages = MiniGameStages.OFF

    Public areSettingsLoaded As Boolean = False

    Public HourlyPay As Integer = 42
    Public MistakePenalty As Integer = 25
    Public MinigameStartTime As Integer = 0
    Public StubTicketIndex As Integer = RND.Next(10, 1050)
    Public NumOfMistakes As Integer = 0

    Public ToggleKey As Keys = Keys.B
    Public NextTaskTriggerTime As Integer = 0
    Public MessageDisplayTime As Integer = 8000

    Public TriggerLocation As New Vector3(-1223.3, -207.7, 39.3)
    Public TriggerHeading As Integer = 253
    Public TriggerBlip As Blip
    Public HoldingPosition As New Vector3(-1239.6, -156.26, 40.41)
    Public WaitingPosition As New Vector3(-1212.4, -186.3, 39.3)
    Public WaitingHeading As Integer = 105

    Public ValetPed As Ped
    Public ValetPedSpawnPoint As New Vector3(-1246.3, -219.8, 40.7)
    Public ValetPedSpawnHeading As Integer = 204

    Public GarageBlip As Blip
    Public DropoffBlip As Blip

    Public UI_Trigger_width As Integer = 280
    Public UI_Trigger_height As Integer = 35
    Public UI_Trigger_x As Integer = (1280 / 2) - (UI_Trigger_width / 2)
    Public UI_Trigger_y As Integer = (720 / 2) - (UI_Trigger_height / 2)
    Public UI_Trigger As New UIContainer(New Point(UI_Trigger_x, UI_Trigger_y), New Size(UI_Trigger_width, UI_Trigger_height), Color.FromArgb(0, 0, 0, 0)) With {.Enabled = False}

    Public DebugMode As Boolean = False
    Public UI_Debug_width As Integer = 180
    Public UI_Debug_height As Integer = 200
    Public UI_Debug As New UIContainer(New Point(1280 - UI_Debug_width, 150), New Size(UI_Debug_width, UI_Debug_height), Color.FromArgb(170, 0, 0, 0))

    Public Shared CurrentCustomer As Customer
    Public EntourageGroup As Integer

    Public IN_5_Timer As Integer = 0
    Public OUT_2_Timer As Integer = 0
    Public OUT_9_Timer As Integer = 0
    Public BACK_2_Timer As Integer = 0

    Public Enum MiniGameStages
        OFF
        WaitingForNextTask

        IN_0_WaitingToClearSpawn
        IN_1
        IN_2
        IN_3
        IN_4
        IN_5
        IN_6
        IN_7
        IN_8
        IN_9
        IN_10
        IN_11
        IN_12

        OUT_1
        OUT_2
        OUT_3
        OUT_4
        OUT_5
        OUT_6
        OUT_7
        OUT_8
        OUT_9
        OUT_10

        BACK_1
        BACK_2
        BACK_3
        BACK_4
    End Enum



    Public Sub New()
        CurrentMinigameStage = MiniGameStages.OFF
        areSettingsLoaded = False

        EntourageGroup = World.AddRelationShipGroup("Entourage")
        World.SetRelationshipBetweenGroups(Relationship.Respect, EntourageGroup, Game.Player.Character.RelationshipGroup)
        World.SetRelationshipBetweenGroups(Relationship.Respect, Game.Player.Character.RelationshipGroup, EntourageGroup)

        createTriggerBlip()

        clearAllActiveBlips()
    End Sub

    Public Sub clearAllActiveBlips()
        Dim ActiveBlips() As Blip
        ActiveBlips = World.GetActiveBlips

        For Each b As Blip In ActiveBlips
            b.Remove()
        Next
    End Sub

    Public Sub createTriggerBlip()
        TriggerBlip = World.CreateBlip(TriggerLocation)
        TriggerBlip.Sprite = 272
        GTA.Native.Function.Call(Native.Hash.SET_BLIP_AS_SHORT_RANGE, TriggerBlip.Handle, True)
    End Sub

    Public Sub checkIfItsTimeToLoadSettings()
        If areSettingsLoaded = True Then Exit Sub
        LoadSettings()
    End Sub

    Public Sub LoadSettings()

        Dim val1, val2, val3, val4 As String
        val1 = Settings.GetValue("SETTINGS", "TOGGLE", "B")
        val2 = Settings.GetValue("SETTINGS", "HOURLYPAY", "42")
        val3 = Settings.GetValue("SETTINGS", "MISTAKEPENALTY", "50")
        val4 = Settings.GetValue("SETTINGS", "SHOWDEBUG", "0")

        Select Case val1
            Case "A"
                ToggleKey = Keys.A
            Case "B"
                ToggleKey = Keys.B
            Case "C"
                ToggleKey = Keys.C
            Case "D"
                ToggleKey = Keys.D
            Case "E"
                ToggleKey = Keys.E
            Case "F"
                ToggleKey = Keys.F
            Case "G"
                ToggleKey = Keys.G
            Case "H"
                ToggleKey = Keys.H
            Case "I"
                ToggleKey = Keys.I
            Case "J"
                ToggleKey = Keys.J
            Case "K"
                ToggleKey = Keys.K
            Case "L"
                ToggleKey = Keys.L
            Case "M"
                ToggleKey = Keys.M
            Case "N"
                ToggleKey = Keys.N
            Case "O"
                ToggleKey = Keys.O
            Case "P"
                ToggleKey = Keys.P
            Case "Q"
                ToggleKey = Keys.Q
            Case "R"
                ToggleKey = Keys.R
            Case "S"
                ToggleKey = Keys.S
            Case "T"
                ToggleKey = Keys.T
            Case "U"
                ToggleKey = Keys.U
            Case "V"
                ToggleKey = Keys.V
            Case "W"
                ToggleKey = Keys.W
            Case "X"
                ToggleKey = Keys.X
            Case "Y"
                ToggleKey = Keys.Y
            Case "Z"
                ToggleKey = Keys.Z
            Case "F1"
                ToggleKey = Keys.F1
            Case "F2"
                ToggleKey = Keys.F2
            Case "F3"
                ToggleKey = Keys.F3
            Case "F4"
                ToggleKey = Keys.F4
            Case "F5"
                ToggleKey = Keys.F5
            Case "F6"
                ToggleKey = Keys.F6
            Case "F7"
                ToggleKey = Keys.F7
            Case "F8"
                ToggleKey = Keys.F8
            Case "F9"
                ToggleKey = Keys.F9
            Case "F10"
                ToggleKey = Keys.F10
            Case "F11"
                ToggleKey = Keys.F11
            Case "F12"
                ToggleKey = Keys.F12
            Case "1"
                ToggleKey = Keys.D1
            Case "2"
                ToggleKey = Keys.D2
            Case "3"
                ToggleKey = Keys.D3
            Case "4"
                ToggleKey = Keys.D4
            Case "5"
                ToggleKey = Keys.D5
            Case "6"
                ToggleKey = Keys.D6
            Case "7"
                ToggleKey = Keys.D7
            Case "8"
                ToggleKey = Keys.D8
            Case "9"
                ToggleKey = Keys.D9
            Case "0"
                ToggleKey = Keys.D0
            Case Else
                ToggleKey = Keys.B
        End Select

        HourlyPay = CInt(val2)
        MistakePenalty = CInt(val3)
        DebugMode = CInt(val4)

        areSettingsLoaded = True
    End Sub

    '=================
    '  UI
    '=================
    Public Sub drawUI()
        drawUI_Trigger()
        drawUI_Debug()
    End Sub

    Public Sub drawUI_Trigger()
        UI_Trigger.Items.Clear()
        If isMinigameActive = True Then
            UI_Trigger.Items.Add(New UIRectangle(New Point(0, 0), New Size(UI_Trigger_width, UI_Trigger_height), Color.OrangeRed))
            UI_Trigger.Items.Add(New UIRectangle(New Point(2, 2), New Size(UI_Trigger_width - 4, UI_Trigger_height - 4), Color.Black))
            UI_Trigger.Items.Add(New UIText("Press " & ToggleKey.ToString & " to end the valet missions", New Point(UI_Trigger_width / 2, 6), 0.4, Color.OrangeRed, 0, True))
        Else
            UI_Trigger.Items.Add(New UIRectangle(New Point(0, 0), New Size(UI_Trigger_width, UI_Trigger_height), Color.GreenYellow))
            UI_Trigger.Items.Add(New UIRectangle(New Point(2, 2), New Size(UI_Trigger_width - 4, UI_Trigger_height - 4), Color.Black))
            UI_Trigger.Items.Add(New UIText("Press " & ToggleKey.ToString & " to start the valet missions", New Point(UI_Trigger_width / 2, 6), 0.4, Color.GreenYellow, 0, True))
        End If
        UI_Trigger.Draw()
    End Sub





    '=================
    '  LOOP & CHECKS
    '=================

    Public Sub Update(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Tick

        checkIfItsTimeToLoadSettings()

        checkIfPlayerIsAtTriggerLocation()
        checkIfPlayerIsWithinPlayingArea()
        checkIfPlayerIsAlive()
        checkIfPlayerIsWanted()
        checkIfAnyCustomersHaveDied()

        checkIfItsTimeToTriggerTheNextEvent()

        check_IN0_Conditions()
        check_IN1_Conditions()
        check_IN2_Conditions()
        check_IN3_Conditions()
        check_IN4_Conditions()
        check_IN5_Conditions()
        check_IN6_Conditions()
        check_IN7_Conditions()
        check_IN8_Conditions()
        check_IN9_Conditions()

        check_OUT1_Conditions()
        check_OUT2_Conditions()
        check_OUT3_Conditions()
        check_OUT4_Conditions()
        check_OUT5_Conditions()
        check_OUT6_Conditions()
        check_OUT7_Conditions()
        check_OUT8_Conditions()
        check_OUT9_Conditions()
        check_OUT10_Conditions()

        check_BACK1_Conditions()
        check_BACK2_Conditions()
        check_BACK3_Conditions()
        check_BACK4_Conditions()

        UpdateAmbientEvents()

        drawUI()
    End Sub

    Public Sub checkIfPlayerIsAtTriggerLocation()
        Dim ppos As Vector3 = Game.Player.Character.Position
        Dim dist As Single = World.GetDistance(ppos, TriggerLocation)

        If dist < 2.5 Then
            If Game.Player.Character.Heading > TriggerHeading - 40 And Game.Player.Character.Heading < TriggerHeading + 40 Then
                UI_Trigger.Enabled = True
            Else
                UI_Trigger.Enabled = False
            End If
        Else
            UI_Trigger.Enabled = False
        End If

    End Sub

    Public Sub checkIfPlayerIsWithinPlayingArea()

        If isMinigameActive = False Then Exit Sub

        Dim radius As Integer = 125
        Dim dist As Single = World.GetDistance(Game.Player.Character.Position, TriggerLocation)

        If dist > radius Then
            If Game.Player.Character.IsInVehicle Then
                TerminateMinigame("joyride")
            Else
                TerminateMinigame("leftarea")
            End If
        End If

    End Sub

    Public Sub checkIfPlayerIsAlive()
        If isMinigameActive = False Then Exit Sub

        If Game.Player.Character.IsDead Then
            TerminateMinigame()
        End If
    End Sub

    Public Sub checkIfPlayerIsWanted()
        If isMinigameActive = False Then Exit Sub

        If Game.Player.WantedLevel > 0 Then
            TerminateMinigame("wanted")
        End If
    End Sub

    Public Sub checkIfAnyCustomersHaveDied()
        Dim PendingRemoval As New List(Of Customer)
        For Each c As Customer In ListOfCustomers
            If c.Entity.IsDead Then

                If c.EntityBlip IsNot Nothing Then
                    If c.EntityBlip.Exists Then
                        c.EntityBlip.Remove()
                    End If
                End If

                If c.CarBlip IsNot Nothing Then
                    If c.CarBlip.Exists Then
                        c.CarBlip.Remove()
                    End If
                End If

                If c.Car IsNot Nothing Then
                    If c.Car.Exists Then
                        c.Car.MarkAsNoLongerNeeded()
                    End If
                End If

                c.Entity.MarkAsNoLongerNeeded()

                PendingRemoval.Add(c)

                If c.Entity.HasBeenDamagedBy(Game.Player.Character) Then
                    TerminateMinigame("killer")
                    Exit Sub
                End If
            End If
        Next

        For Each c As Customer In PendingRemoval
            ListOfCustomers.Remove(c)
        Next
    End Sub

    '=======
    'TRIGGER
    '=======
    Public Sub checkIfItsTimeToTriggerTheNextEvent()
        If CurrentMinigameStage <> MiniGameStages.WaitingForNextTask Then Exit Sub
        If Game.GameTime < NextTaskTriggerTime Then Exit Sub

        ' All conditions met

        Dim r As Integer = 0
        Dim count As Integer = 0
        count = ListOfCustomers.Count

        If count > 2 Then
            r = RND.Next(0, 4 + 1)
        End If

        Select Case r
            Case Is < 2
                Start_IN_Sequence()
            Case Else
                Start_OUT_Sequence()
        End Select

    End Sub


    '==================
    ' CUSTOMER ARRIVING
    '==================

    Public Sub Start_IN_Sequence()
        CurrentMinigameStage = MiniGameStages.IN_0_WaitingToClearSpawn
    End Sub

    'If spawn area is clear of vehicles
    'Create new customer
    Public Sub check_IN0_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_0_WaitingToClearSpawn Then Exit Sub

        If AmbientEvents.Car IsNot Nothing Then
            Dim d As Single = AmbientEvents.Car.Position.DistanceTo(CarSpawnPoint1)
            DebugMessage = "Distance from other managed car: " & Math.Round(d, 1)
            If d < 15 Then Exit Sub
        End If

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(CarSpawnPoint1)
        If dist < 55 Then Exit Sub

        'ALL CONDITIONS MET

        CurrentCustomer = New Customer
        MissionVehicleSpawnTime = Game.GameTime

        CurrentMinigameStage = MiniGameStages.IN_1
    End Sub

    'If customer ped is entering rotunda area
    'Show blip on map, flash map
    Public Sub check_IN1_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_1 Then Exit Sub

        Dim dist As Single
        dist = CurrentCustomer.Entity.Position.DistanceTo(DropOffPoint)
        If dist > 50 Then Exit Sub

        ' ALL CONDITIONS MET

        GTA.Native.Function.Call(Native.Hash.FLASH_MINIMAP_DISPLAY)

        CurrentCustomer.CarBlip = CurrentCustomer.Car.AddBlip
        CurrentCustomer.CarBlip.Sprite = 227
        CurrentCustomer.CarBlip.Color = BlipColor.Green

        CurrentMinigameStage = MiniGameStages.IN_2
    End Sub

    'If customer ped has driven to the dropoff point
    'Customer ped exits vehicle
    Public Sub check_IN2_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_2 Then Exit Sub

        Dim dist As Single
        dist = CurrentCustomer.Entity.Position.DistanceTo(DropOffPoint)
        If dist > 8 Then Exit Sub

        If CurrentCustomer.Car.Speed > 0.05 Then Exit Sub

        ' ALL CONDITIONS MET

        Dim task As New TaskSequence
        task.AddTask.Wait(RND.Next(850, 2300))
        task.AddTask.LeaveVehicle(CurrentCustomer.Car, False)
        task.Close()
        CurrentCustomer.Entity.Task.PerformSequence(task)

        If CurrentCustomer.Entourage.Count > 0 Then
            For Each p As Ped In CurrentCustomer.Entourage
                Dim ts As New TaskSequence
                ts.AddTask.Wait(RND.Next(500, 1600))
                ts.AddTask.LeaveVehicle(CurrentCustomer.Car, True)
                p.Task.PerformSequence(ts)
            Next
        End If

        CurrentCustomer.CarBlip.IsFlashing = True

        CurrentMinigameStage = MiniGameStages.IN_3
    End Sub

    'If customer ped has exited the vehicle
    'Customer ped turns to face player
    Public Sub check_IN3_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_3 Then Exit Sub

        If CurrentCustomer.Entity.IsInVehicle = True Then Exit Sub

        ' ALL CONDITIONS MET

        CurrentCustomer.Entity.Task.TurnTo(Game.Player.Character)
        GTA.UI.ShowSubtitle("Walk up to the ~g~customer~w~.", MessageDisplayTime)

        If CurrentCustomer.Entourage.Count > 0 Then
            For Each p As Ped In CurrentCustomer.Entourage
                p.Task.TurnTo(Game.Player.Character)
            Next
        End If

        CurrentMinigameStage = MiniGameStages.IN_4
    End Sub

    'If player has reached customer ped
    '"Get Keys" and wait
    Public Sub check_IN4_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_4 Then Exit Sub

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(CurrentCustomer.Entity.Position)
        If dist > 4 Then Exit Sub

        ' ALL CONDITIONS MET

        GTA.UI.ShowSubtitle("Here are the keys.", MessageDisplayTime)
        IN_5_Timer = Game.GameTime + 1200

        CurrentMinigameStage = MiniGameStages.IN_5
    End Sub

    'If keys have been "handed over"
    'Customer ped walks away
    Public Sub check_IN5_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_5 Then Exit Sub

        If Game.GameTime < IN_5_Timer Then Exit Sub

        ' ALL CONDITIONS MET

        CurrentCustomer.DropoffTime = Game.GameTime
        CurrentCustomer.Entity.Task.GoTo(HoldingPosition)
        GTA.UI.ShowSubtitle("Get in the customer's ~g~car~w~.")

        If CurrentCustomer.Entourage.Count > 0 Then
            For Each p As Ped In CurrentCustomer.Entourage
                Dim offsetVector As New Vector3(RND.Next(-40, 40) / 10, RND.Next(-40, 40) / 10, 0)
                p.Task.GoTo(HoldingPosition + offsetVector)
            Next
        End If

        CurrentMinigameStage = MiniGameStages.IN_6
    End Sub

    'If player is in customer's car
    'Tell player to drive into garage
    Public Sub check_IN6_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_6 Then Exit Sub

        If Game.Player.Character.IsInVehicle = False Then Exit Sub

        If Game.Player.Character.CurrentVehicle <> CurrentCustomer.Car Then Exit Sub

        ' ALL CONDITIONS MET

        If CurrentCustomer.CarBlip IsNot Nothing And CurrentCustomer.CarBlip.Exists Then CurrentCustomer.CarBlip.Remove()

        CurrentCustomer.CarHealthReceived = CurrentCustomer.Car.Health
        CurrentCustomer.EngineHealthReceived = CurrentCustomer.Car.EngineHealth
        CurrentCustomer.TankHealthReceived = CurrentCustomer.Car.PetrolTankHealth

        StubTicketIndex += 1
        CurrentCustomer.ValetStub = StubTicketIndex

        GTA.UI.ShowSubtitle("This is car #" & StubTicketIndex & ". Drive it into the ~y~garage~w~.", MessageDisplayTime)

        GarageBlip = World.CreateBlip(GarageEntrance)
        GarageBlip.Color = BlipColor.Yellow

        CurrentMinigameStage = MiniGameStages.IN_7
    End Sub

    'If player has reached garage
    'Tell player to park the car
    Public Sub check_IN7_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_7 Then Exit Sub

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(GarageEntrance)
        If dist > 10 Then Exit Sub

        ' ALL CONDITIONS MET

        If GarageBlip IsNot Nothing And GarageBlip.Exists Then GarageBlip.Remove()

        GTA.UI.ShowSubtitle("Park the car.", MessageDisplayTime)

        CurrentMinigameStage = MiniGameStages.IN_8
    End Sub

    'If player has exited vehicle
    'Tell player to return to the rotunda
    Public Sub check_IN8_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_8 Then Exit Sub

        If Game.Player.Character.IsInVehicle = True Then Exit Sub

        ' ALL CONDITIONS MET

        GTA.UI.ShowSubtitle("Return to the valet drop-off area.", MessageDisplayTime)

        If CurrentCustomer.Entourage.Count > 0 Then
            For Each p As Ped In CurrentCustomer.Entourage
                p.MarkAsNoLongerNeeded()
                p.Delete()
            Next
            CurrentCustomer.Entourage.Clear()
        End If

        CurrentCustomer.Car.PreviouslyOwnedByPlayer = True

        CurrentCustomer = Nothing

        CurrentMinigameStage = MiniGameStages.IN_9
    End Sub

    'If player has reached rotunda
    'Start new loop
    Public Sub check_IN9_Conditions()
        If CurrentMinigameStage <> MiniGameStages.IN_9 Then Exit Sub

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(DropOffPoint)
        If dist > 30 Then Exit Sub

        ' ALL CONDITIONS MET

        NextTaskTriggerTime = Game.GameTime + RND.Next(3000, 9000)
        CurrentMinigameStage = MiniGameStages.WaitingForNextTask
    End Sub




    '=================
    ' CUSTOMER LEAVING
    '=================

    Public Sub Start_OUT_Sequence()

        Dim i As Integer = RND.Next(0, ListOfCustomers.Count - 2)
        CurrentCustomer = ListOfCustomers(i)

        CurrentCustomer.EntityBlip = CurrentCustomer.Entity.AddBlip
        CurrentCustomer.EntityBlip.Color = BlipColor.Blue
        CurrentCustomer.EntityBlip.Scale = 0.75

        CurrentCustomer.Entity.RelationshipGroup = EntourageGroup

        Dim numOfSeats As Integer = GTA.Native.Function.Call(Of Integer)(Native.Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, CurrentCustomer.Car)
        Dim numOfPax As Integer = RND.Next(0, numOfSeats + 1)

        'If numOfPax > 2 Then
        '    Dim p3 As Ped
        '    p3 = World.CreateRandomPed(HoldingPosition)
        '    CurrentCustomer.Entourage.Add(p3)
        'p3.RelationshipGroup = EntourageGroup
        'End If

        'If numOfPax > 1 Then
        'Dim p2 As Ped
        'p2 = World.CreateRandomPed(HoldingPosition)
        'CurrentCustomer.Entourage.Add(p2)
        'p2.RelationshipGroup = EntourageGroup
        'End If

        'If numOfPax > 0 Then
        'Dim p1 As Ped
        'p1 = World.CreateRandomPed(HoldingPosition)
        'CurrentCustomer.Entourage.Add(p1)
        'p1.RelationshipGroup = EntourageGroup
        'End If

        CurrentCustomer.Entity.Task.GoTo(Game.Player.Character, Game.Player.Character.ForwardVector * 2.5)

        If CurrentCustomer.Entourage IsNot Nothing Then
            For Each p As Ped In CurrentCustomer.Entourage
                Dim offset As Vector3
                offset = New Vector3(RND.Next(-20, 20) / 10, RND.Next(-20, 20) / 10, 0)
                p.Task.GoTo(CurrentCustomer.Entity, offset)
            Next
        End If

        CurrentMinigameStage = MiniGameStages.OUT_1
    End Sub

    'If customer ped has reached player's position
    'Request car, wait
    Public Sub check_OUT1_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_1 Then Exit Sub

        Dim dist = CurrentCustomer.Entity.Position.DistanceTo(Game.Player.Character.Position) 'World.GetDistance(Game.Player.Character.Position, CurrentCustomer.Entity.Position)
        If dist > 3 Then Exit Sub

        ' All Conditions Met
        CurrentCustomer.Entity.Task.TurnTo(Game.Player.Character)
        Game.Player.Character.Task.TurnTo(CurrentCustomer.Entity, 3000)

        Dim stub As Integer = CurrentCustomer.ValetStub
        Dim txt As String = ""
        Dim r As Integer = RND.Next(0, 8 + 1)

        Select Case r
            Case 0
                txt = "Please get car #" & stub & "."
            Case 1
                txt = "My car is #" & stub & "."
            Case 2
                txt = "I lost my stub. My plate number is " & CurrentCustomer.Car.NumberPlate.ToString & "."
            Case 3
                Dim plate As String = CurrentCustomer.Car.NumberPlate.ToString
                txt = "Where's my stub? I think my plate ends in -" & plate.Substring(plate.Length - RND.Next(2, 5)) & "."
            Case 4
                txt = "I have number " & stub & "."
            Case 5
                txt = "Car " & stub & "."
            Case 6
                txt = "Here's my stub. #" & stub & "."
            Case 7
                txt = "#" & stub & ", please."
            Case 8
                txt = "I spilled a drink on my stub. All I can see is the last number on it: " & stub Mod 10 & "."
            Case Else
                txt = "Number " & stub & ", please."
        End Select

        If CurrentCustomer.Car.IsConvertible = True Then ' And World.Weather <> Weather.Raining And World.Weather <> Weather.ThunderStorm Then
            If CurrentCustomer.Car.RoofState = VehicleRoofState.Closed Then
                r = RND.Next(0, 2)
                If r > 0 Then
                    CurrentCustomer.WantsTopDown = True
                    r = RND.Next(0, 2)
                    If r = 0 Then
                        txt = txt & " And can you put the top down, too?"
                    Else
                        txt = txt & " And open the roof up, please."
                    End If
                End If
            End If
        End If

        GTA.UI.ShowSubtitle(txt, MessageDisplayTime)

        OUT_2_Timer = Game.GameTime + 3200

        CurrentMinigameStage = MiniGameStages.OUT_2
    End Sub

    'If customer ped has waited
    'Ped goes to waiting position
    Public Sub check_OUT2_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_2 Then Exit Sub

        If Game.GameTime < OUT_2_Timer Then Exit Sub

        ' All Conditions Met

        CurrentCustomer.WaitingStartTime = Game.GameTime
        CurrentCustomer.Entity.Task.GoTo(WaitingPosition, False)

        If CurrentCustomer.Entourage IsNot Nothing Then
            For Each p As Ped In CurrentCustomer.Entourage
                Dim offset As Vector3
                offset = New Vector3(RND.Next(-60, 60) / 10, RND.Next(-60, 60) / 10, 0)
                p.Task.GoTo(CurrentCustomer.Entity, offset)
                p.Task.GoTo(CurrentCustomer.Entity, offset)
            Next
        End If

        CurrentMinigameStage = MiniGameStages.OUT_3
    End Sub

    'If player has entered the garage
    'Tell player to retrieve car
    Public Sub check_OUT3_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_3 Then Exit Sub

        Dim dist As Single = Game.Player.Character.Position.DistanceTo(GarageEntrance)
        If dist > 10 Then Exit Sub

        ' All Conditions Met

        GTA.UI.ShowSubtitle("Retrieve the customer's car.", MessageDisplayTime)
        CurrentMinigameStage = MiniGameStages.OUT_4
    End Sub

    'If player has entered a car
    'Tell player to drive it to the rotunda
    Public Sub check_OUT4_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_4 Then Exit Sub

        If Game.Player.Character.IsInVehicle = False Then Exit Sub

        ' All Conditions Met

        If CurrentCustomer.EntityBlip IsNot Nothing Then
            If CurrentCustomer.EntityBlip.Exists Then
                CurrentCustomer.EntityBlip.Remove()
            End If
        End If

        GTA.UI.ShowSubtitle("Drive the car to the ~b~customer~w~.", MessageDisplayTime)
        CurrentCustomer.Entity.Heading = 105

        DropoffBlip = World.CreateBlip(DropOffPoint)
        DropoffBlip.Color = BlipColor.Blue

        CurrentMinigameStage = MiniGameStages.OUT_5
    End Sub

    'If player has arrived at rotunda
    'Wait for player to get out of the car
    Public Sub check_OUT5_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_5 Then Exit Sub

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(DropOffPoint)
        If dist > 7 Then Exit Sub

        ' All Conditions Met
        If DropoffBlip IsNot Nothing Then
            If DropoffBlip.Exists Then
                DropoffBlip.Remove()
            End If
        End If

        GTA.UI.ShowSubtitle("Exit the car.", MessageDisplayTime)

        CurrentMinigameStage = MiniGameStages.OUT_6
    End Sub

    'If player has exited car
    'Check if it's the right car
    Public Sub check_OUT6_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_6 Then Exit Sub

        If Game.Player.Character.IsInVehicle = True Then Exit Sub

        ' All Conditions Met

        If Game.Player.LastVehicle = CurrentCustomer.Car Then
            CurrentMinigameStage = MiniGameStages.OUT_7
        Else
            CurrentMinigameStage = MiniGameStages.BACK_1
        End If

    End Sub

    'If player got out of correct car
    'Have customer ped walk to player
    Public Sub check_OUT7_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_7 Then Exit Sub

        ' All Conditions Met

        CurrentCustomer.Entity.Task.GoTo(Game.Player.Character)

        If CurrentCustomer.Entourage IsNot Nothing Then
            Dim i As Integer = 0
            For Each p As Ped In CurrentCustomer.Entourage
                If i = 0 Then
                    p.Task.EnterVehicle(CurrentCustomer.Car, VehicleSeat.RightFront, 12000)
                ElseIf i = 1 Then
                    p.Task.EnterVehicle(CurrentCustomer.Car, VehicleSeat.RightRear, 12000)
                ElseIf i = 2 Then
                    p.Task.EnterVehicle(CurrentCustomer.Car, VehicleSeat.LeftRear, 12000)
                End If
                i += 1
            Next
        End If

        CurrentMinigameStage = MiniGameStages.OUT_8
    End Sub

    'If customer ped has reached player
    'Say thank you and tip, wait
    Public Sub check_OUT8_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_8 Then Exit Sub

        Dim dist As Single
        dist = CurrentCustomer.Entity.Position.DistanceTo(Game.Player.Character.Position)
        If dist > 4 Then Exit Sub

        ' All Conditions Met

        CurrentCustomer.Entity.Task.TurnTo(Game.Player.Character)
        Game.Player.Character.Task.TurnTo(CurrentCustomer.Entity, 3000)

        Dim tip As Integer
        If CurrentCustomer.Car.Health < CurrentCustomer.CarHealthReceived - 1 Or CurrentCustomer.Car.EngineHealth < CurrentCustomer.EngineHealthReceived - 1 Or CurrentCustomer.Car.PetrolTankHealth < CurrentCustomer.TankHealthReceived - 1 Then
            GTA.UI.ShowSubtitle("You scratched it! How dare you!", MessageDisplayTime)
            tip = 0
            NumOfMistakes += 1
            CurrentCustomer.MistakesExperienced += 1
        ElseIf CurrentCustomer.WaitingStartTime + 60000 < Game.GameTime Then
            GTA.UI.ShowSubtitle("That took a while...", MessageDisplayTime)
            tip = RND.Next(50, 150)
            NumOfMistakes += 1
            CurrentCustomer.MistakesExperienced += 1
        ElseIf CurrentCustomer.LeftEngineRunning = True Then
            GTA.UI.ShowSubtitle("Why is my fuel level so low? Did you leave the engine running?!", MessageDisplayTime)
            tip = 0
            NumOfMistakes += 1
            CurrentCustomer.MistakesExperienced += 1
        ElseIf CurrentCustomer.WantsTopDown = True And CurrentCustomer.Car.RoofState = VehicleRoofState.Closed Then
            GTA.UI.ShowSubtitle("I told you to open the roof. Why didn't you listen?", MessageDisplayTime)
            tip = 0
        Else
            Dim txt As String = ""
            Dim r As Integer = RND.Next(0, 10 + 1)

            Select Case r
                Case 0
                    txt = "Thank you."
                Case 1
                    txt = "Great. Have a nice day!"
                Case 2
                    txt = "Looks good. Here's your tip."
                Case 3
                    txt = "Thanks, buddy. See ya 'round."
                Case 4
                    If CurrentCustomer.Entity.Gender = Gender.Male Then
                        txt = "Thanks, dude."
                    Else
                        If Game.Player.Character.Gender = Gender.Male Then
                            txt = "Thanks, handsome."
                        Else
                            txt = "Thank you."
                        End If
                    End If
                Case 4
                    If Game.Player.Character.Gender = Gender.Male Then
                        txt = "Thank you, sir."
                    Else
                        txt = "Thank you, miss."
                    End If
                Case 5
                    If CurrentCustomer.Entity.Gender = Gender.Male Then
                        If Game.Player.Character.Gender = Gender.Male Then
                            txt = "Bro, you're awesome."
                        Else
                            txt = "Baby, you're awesome."
                        End If
                    Else
                        If Game.Player.Character.Gender = Gender.Male Then
                            txt = "Dude, you're like, totally rad! Thanks, hun."
                        Else
                            txt = "Thanks, sister!"
                        End If
                    End If
                Case 6
                    txt = "Excellent; well done!"
                Case 7
                    txt = "Thanks, enjoy your day!"
                Case 9
                    txt = "Don't spend it all at once, okay?"
                Case 10
                    txt = "Thanks for takin' car of my car."
                Case Else
                    txt = "Well done."
            End Select

            GTA.UI.ShowSubtitle(txt, MessageDisplayTime)
            tip = RND.Next(50, 150)
        End If

        tip = CInt(tip / CurrentCustomer.MistakesExperienced)
        If tip = 0 Then
            GTA.UI.Notify("You didn't receive a tip.")
        Else
            GTA.UI.Notify("$" & tip & " tip received.")
            Game.Player.Money += tip
        End If

        If CurrentCustomer.EntityBlip IsNot Nothing And CurrentCustomer.EntityBlip.Exists Then CurrentCustomer.EntityBlip.Remove()

        OUT_9_Timer = Game.GameTime + 3000
        CurrentMinigameStage = MiniGameStages.OUT_9
    End Sub

    'If customer has tipped player and waited 2s
    'Customer ped enters car and drives away
    Public Sub check_OUT9_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_9 Then Exit Sub

        If Game.GameTime < OUT_9_Timer Then Exit Sub

        ' All conditions met

        CurrentCustomer.Car.IsStolen = False
        CurrentCustomer.Car.IsWanted = False

        CurrentCustomer.Entity.Task.DriveTo(CurrentCustomer.Car, AlleyEntranceNorth, 5, CurrentCustomer.DrivingSpeed, 1)

        CurrentMinigameStage = MiniGameStages.OUT_10
    End Sub

    'If customer ped has entered car
    'Forget the customer. Remove any leftover blips.
    Public Sub check_OUT10_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_10 Then Exit Sub

        If CurrentCustomer.Entity.IsInVehicle = False Then Exit Sub
        If CurrentCustomer.Entity.CurrentVehicle <> CurrentCustomer.Car Then Exit Sub

        ' All conditions met

        ListOfCustomers.Remove(CurrentCustomer)

        If CurrentCustomer.Entourage IsNot Nothing Then
            Dim i As Integer = 0
            For Each p As Ped In CurrentCustomer.Entourage
                p.MarkAsNoLongerNeeded()
            Next
        End If

        If CurrentCustomer.EntityBlip IsNot Nothing And CurrentCustomer.EntityBlip.Exists Then CurrentCustomer.EntityBlip.Remove()
        If CurrentCustomer.CarBlip IsNot Nothing And CurrentCustomer.CarBlip.Exists Then CurrentCustomer.CarBlip.Remove()

        CurrentCustomer.Car.MarkAsNoLongerNeeded()
        CurrentCustomer.Entity.MarkAsNoLongerNeeded()
        CurrentCustomer = Nothing

        NextTaskTriggerTime = Game.GameTime + RND.Next(3000, 9000)
        CurrentMinigameStage = MiniGameStages.WaitingForNextTask
    End Sub




    '====================
    ' TRY AGAIN RETRIEVAL
    '====================

    'If Player brought the wrong car
    'Customer complains
    Public Sub check_BACK1_Conditions()
        If CurrentMinigameStage <> MiniGameStages.BACK_1 Then Exit Sub

        ' ALL CONDITIONS MET

        CurrentCustomer.MistakesExperienced += 1

        GTA.UI.ShowSubtitle("That's not my car.", 3000)
        BACK_2_Timer = Game.GameTime + 3000

        NumOfMistakes += 1

        CurrentMinigameStage = MiniGameStages.BACK_2
    End Sub

    'If player has been told about wrong car
    'Tell player to take it back to the garage
    Public Sub check_BACK2_Conditions()
        If CurrentMinigameStage <> MiniGameStages.BACK_2 Then Exit Sub

        If Game.GameTime < BACK_2_Timer Then Exit Sub

        ' ALL CONDITIONS MET

        GTA.UI.ShowSubtitle("Drive the car back into the ~y~garage~w~.", MessageDisplayTime)

        GarageBlip = World.CreateBlip(GarageEntrance, 2)
        GarageBlip.Color = BlipColor.Yellow

        CurrentMinigameStage = MiniGameStages.BACK_3
    End Sub

    'If player has reached garage
    'Tell player to park the car
    Public Sub check_BACK3_Conditions()
        If CurrentMinigameStage <> MiniGameStages.BACK_3 Then Exit Sub

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(GarageEntrance)
        If dist > 10 Then Exit Sub

        ' ALL CONDITIONS MET

        If GarageBlip IsNot Nothing And GarageBlip.Exists Then GarageBlip.Remove()

        GTA.UI.ShowSubtitle("Park the car.", MessageDisplayTime)

        CurrentMinigameStage = MiniGameStages.BACK_4
    End Sub

    'If player has exited car
    'Tell player to get into the customer's car.
    Public Sub check_BACK4_Conditions()
        If CurrentMinigameStage <> MiniGameStages.BACK_4 Then Exit Sub

        If Game.Player.Character.IsInVehicle = True Then Exit Sub

        ' ALL CONDITIONS MET

        GTA.UI.ShowSubtitle("Find and retrieve the correct car.", MessageDisplayTime)
        CurrentMinigameStage = MiniGameStages.OUT_4
    End Sub





    '=================
    '  ON & OFF
    '=================

    Public Sub ToggleMinigame(ByVal sender As Object, ByVal k As KeyEventArgs) Handles MyBase.KeyUp
        If k.KeyCode = ToggleKey Then
            If UI_Trigger.Enabled = True Then
                If isMinigameActive = True Then
                    TerminateMinigame("welldone")
                Else
                    BeginMinigame()
                End If
            End If
        End If
    End Sub

    Public Sub BeginMinigame()
        isMinigameActive = True
        MinigameStartTime = Game.GameTime
        NumOfMistakes = 0

        'ValetPed = World.CreatePed("s_m_y_valet_01", ValetPedSpawnPoint, ValetPedSpawnHeading)

        NextTaskTriggerTime = Game.GameTime + RND.Next(2000, 4000)
        CurrentMinigameStage = MiniGameStages.WaitingForNextTask

        StartAmbientEvents()

    End Sub

    Public Sub TerminateMinigame(Optional condition As String = "")
        Select Case condition
            Case "leftarea"
                GTA.UI.Notify("Valet missions terminated. You left the area.")
            Case "joyride"
                For Each c As Customer In ListOfCustomers
                    If c.Car = Game.Player.Character.CurrentVehicle Then
                        GTA.UI.Notify("The hotel called the cops because you stole a customer's vehicle!")
                        Game.Player.WantedLevel = 2
                        Game.Player.Character.CurrentVehicle.IsWanted = True
                        Exit For
                    Else
                        GTA.UI.Notify("Valet missions terminated. You left the area.")
                    End If
                Next
            Case "killer"
                GTA.UI.Notify("Why are you hurting our customers? Get out of here!")
            Case "wanted"
                GTA.UI.Notify("We don't employ criminals. Get out of here!")
            Case "welldone"
                Dim n As String = ""
                Select Case Game.Player.Character.Model
                    Case "player_zero"
                        n = ", Michael"
                    Case "player_one"
                        n = ", Franklin"
                    Case "player_two"
                        n = ", Trevor"
                End Select

                Dim wage As Integer = Math.Round(((Game.GameTime - MinigameStartTime) / 2000 / 60) * HourlyPay)
                Dim txt As String = ""

                If NumOfMistakes = 0 Then
                    txt = "Nice work" & n & "! You earned $" & wage & "."
                ElseIf NumOfMistakes = 1 Then
                    wage -= (NumOfMistakes * MistakePenalty)
                    txt = "Not too bad. You only made one mistake this shift (-$" & MistakePenalty & " penalty)."
                Else
                    wage -= (NumOfMistakes * MistakePenalty)
                    txt = "Try to improve next time" & n & ". You made " & NumOfMistakes & " mistakes this shift (-$" & NumOfMistakes * MistakePenalty & " penalty)."
                End If
                GTA.UI.Notify(txt)
                Game.Player.Money += wage
            Case Else
                GTA.UI.Notify("Valet minigame ended.")
        End Select

        isMinigameActive = False

        DismissAllEntities()

        CurrentMinigameStage = MiniGameStages.OFF

        EndAmbientEvents()
    End Sub

    Public Sub DismissAllEntities()
        DebugMessage = "Dismiss Start"
        If ListOfCustomers.Count > 0 Then
            For Each c As Customer In ListOfCustomers
                DebugMessage = "Dismiss 1"
                If c IsNot Nothing Then
                    If c.Entity IsNot Nothing Then
                        If c.Entity.Exists Then

                            DebugMessage = "Dismiss 2"
                            If c.EntityBlip IsNot Nothing Then
                                If c.EntityBlip.Exists Then
                                    c.EntityBlip.Remove()
                                End If
                            End If

                            DebugMessage = "Dismiss 3"
                            If c.CarBlip IsNot Nothing Then
                                If c.CarBlip.Exists Then
                                    c.CarBlip.Remove()
                                End If
                            End If

                            DebugMessage = "Dismiss 4"
                            If c.Car IsNot Nothing Then
                                If c.Car.Exists Then
                                    c.Car.MarkAsNoLongerNeeded()
                                End If
                            End If

                            DebugMessage = "Dismiss 5"
                            If c.Entourage IsNot Nothing Then
                                If c.Entourage.Count > 0 Then
                                    For Each p As Ped In c.Entourage
                                        If p.CurrentBlip IsNot Nothing Then
                                            If p.CurrentBlip.Exists Then
                                                p.CurrentBlip.Remove()
                                            End If
                                        End If
                                        p.MarkAsNoLongerNeeded()
                                    Next
                                End If
                            End If

                            DebugMessage = "Dismiss 6"
                            c.Entity.MarkAsNoLongerNeeded()
                        End If
                    End If
                End If

            Next
        End If

        If GarageBlip IsNot Nothing Then
            If GarageBlip.Exists Then
                GarageBlip.Remove()
            End If
        End If

        If DropoffBlip IsNot Nothing Then
            If DropoffBlip.Exists Then
                DropoffBlip.Remove()
            End If
        End If

        DebugMessage = "Dismiss 7"
        If ValetPed IsNot Nothing Then
            If ValetPed.Exists Then
                ValetPed.MarkAsNoLongerNeeded()
            End If
        End If

        DebugMessage = "Dismiss 8"
        ListOfCustomers.Clear()
        If CurrentCustomer IsNot Nothing Then
            CurrentCustomer = Nothing
        End If

        DebugMessage = "Dismiss Complete"
    End Sub




    '==================================================================================================
    'TEST & DEBUG METHODS

    Public Sub DismissAllKeyChecker(ByVal sender As Object, ByVal k As KeyEventArgs) Handles MyBase.KeyUp
        If DebugMode = False Then Exit Sub
        If k.KeyCode <> Keys.K Then Exit Sub
        DismissAllEntities()
    End Sub

    Public Sub StartAnywhere(ByVal sender As Object, ByVal k As KeyEventArgs) Handles MyBase.KeyUp
        If DebugMode = False Then Exit Sub
        If k.KeyCode <> ToggleKey Then Exit Sub

        If Game.Player.Character.Position.DistanceTo(TriggerLocation) < 5 Then Exit Sub

        If isMinigameActive = True Then
            TerminateMinigame()
        Else
            BeginMinigame()
        End If
    End Sub

    Public Sub drawUI_Debug()
        If DebugMode = False Then Exit Sub


        UI_Debug.Items.Clear()
        Dim row As Integer = -1
        Dim txt As String

        row += 1
        UI_Debug.Items.Add(New UIText(DebugMessage, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Active? " & isMinigameActive.ToString
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Spawn Dist: " & World.GetDistance(Game.Player.Character.Position, AlleyEntranceSouth)
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        If isMinigameActive = True Then
            txt = "On the clock: " & Math.Round((Game.GameTime - MinigameStartTime) / 2000) & " minutes ($" & Math.Round(((Game.GameTime - MinigameStartTime) / 2000 / 60) * HourlyPay) & ")"
        Else
            txt = "Not on the clock"
        End If
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Stage: " & CurrentMinigameStage.ToString
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        If Game.Player.Character.IsInVehicle Then
            For Each c As Customer In ListOfCustomers
                If c.Car = Game.Player.Character.CurrentVehicle Then
                    row += 1
                    txt = "Valet Stub: " & c.ValetStub
                    UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))
                    Exit For
                End If
            Next
        End If

        row += 1
        txt = "Total Customers: " & ListOfCustomers.Count
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        If CurrentCustomer IsNot Nothing Then
            txt = "Curr Cust Car: " & CurrentCustomer.Car.Handle.ToString & " / V " & Math.Round(CurrentCustomer.Car.Speed, 1) & " / D " & CurrentCustomer.Car.Health
        Else
            txt = "Curr Cust Car: No Curr Customer"
        End If
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        If CurrentCustomer IsNot Nothing Then
            txt = "Curr Cust: " & CurrentCustomer.Entity.Handle.ToString
            If CurrentCustomer.Entourage IsNot Nothing Then
                txt = txt & " Entourage size: " & CurrentCustomer.Entourage.Count
            End If
        Else
            txt = "Curr Cust: No Curr Customer"
        End If
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Player in car? " & Game.Player.Character.IsInVehicle.ToString
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        If Game.Player.Character.IsInVehicle Then
            row += 1
            txt = "Car stolen? " & Game.Player.Character.CurrentVehicle.IsStolen.ToString
            UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))
        End If

        row += 1
        txt = "Ambient Stage: " & AmbientStage.ToString
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Game Time: " & Math.Round(Game.GameTime / 1000)
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Ambient Vehicle Spawn Time: " & Math.Round(AmbientVehicleSpawnTime / 1000)
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Mission Vehicle Spawn Time: " & Math.Round(MissionVehicleSpawnTime / 1000)
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Settings: Loaded: " & areSettingsLoaded.ToString & " / Hourly: " & HourlyPay & " / Penalty: " & MistakePenalty
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        UI_Debug.Draw()
    End Sub
End Class


' CUSTOMERS

Public Class Customer
    Public Entity As Ped
    Public EntityBlip As Blip
    Public Car As Vehicle
    Public CarBlip As Blip

    Public DrivingSpeed As Single = RND.Next(60, 150) / 10

    Public CarHealthReceived As Integer = 1000
    Public EngineHealthReceived As Integer = 1000
    Public TankHealthReceived As Integer = 1000
    Public LeftEngineRunning As Boolean = False
    Public MistakesExperienced As Integer = 1
    Public WantsTopDown As Boolean = False

    Public DropoffTime As Integer = 0
    Public WaitingStartTime As Integer = 0
    Public ValetStub As Integer = 0
    Public Entourage As New List(Of Ped)


    Public Sub New()

        ListOfCustomers.Add(Me)

        Dim r As Integer = 0

        Dim i As Integer = RND.Next(0, CarTypes.Length)
        Car = World.CreateVehicle(CarTypes(i), CarSpawnPoint1, CarSpawnPoint1HDG)
        Car.IsStolen = True
        Car.IsWanted = True
        Dim plate As String = RND.Next(0, 10) & RND.Next(0, 10) & UppercaseLetters(RND.Next(0, UppercaseLetters.Length)) & UppercaseLetters(RND.Next(0, UppercaseLetters.Length)) & UppercaseLetters(RND.Next(0, UppercaseLetters.Length)) & RND.Next(0, 10) & RND.Next(0, 10) & RND.Next(0, 10)
        Car.NumberPlate = plate

        'If Car.IsConvertible = True Then ' And World.Weather <> Weather.Raining And World.Weather <> Weather.ThunderStorm Then
        'r = RND.Next(0, 2)
        'If r > 0 Then
        'Car.RoofState = VehicleRoofState.Opened
        'WantsTopDown = True
        'End If
        'End If


        ' CAR MODS
        r = RND.Next(0, 5)
        If r = 0 Then
            GTA.Native.Function.Call(Native.Hash.SET_VEHICLE_MOD_KIT, Car, 0)
            Dim mods() As Integer = [Enum].GetValues(GetType(VehicleMod))
            For Each m As VehicleMod In mods
                Car.SetMod(m, RND.Next(0, 30), True)
            Next

            Dim cols() As Integer = [Enum].GetValues(GetType(VehicleColor))
            r = RND.Next(0, cols.Length)
            Car.PrimaryColor = cols(r)
            Car.SecondaryColor = cols(r)
        End If

        Dim wheels() As Integer = [Enum].GetValues(GetType(VehicleWheelType))
        r = RND.Next(0, wheels.Length)
        Car.WheelType = r

        ' PASSENGERS
        Dim numOfSeats As Integer = GTA.Native.Function.Call(Of Integer)(Native.Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, Car)
        Dim numOfPax As Integer = RND.Next(0, numOfSeats + 1)

        If numOfPax > 2 Then
            Dim p3 As Ped
            r = RND.Next(0, PedTypes.Length)
            p3 = Car.CreatePedOnSeat(VehicleSeat.LeftRear, PedTypes(r))
            Entourage.Add(p3)
        End If

        If numOfPax > 1 Then
            Dim p2 As Ped
            r = RND.Next(0, PedTypes.Length)
            p2 = Car.CreatePedOnSeat(VehicleSeat.RightRear, PedTypes(r))
            Entourage.Add(p2)
        End If

        If numOfPax > 0 Then
            Dim p1 As Ped
            r = RND.Next(0, PedTypes.Length)
            p1 = Car.CreatePedOnSeat(VehicleSeat.RightFront, PedTypes(r))
            Entourage.Add(p1)
        End If

        ' DRIVER
        r = RND.Next(0, PedTypes.Length)
        Entity = Car.CreatePedOnSeat(VehicleSeat.Driver, PedTypes(r))
        Entity.Money = RND.Next(20, 200)

        Dim offsetVector As New Vector3(RND.Next(-20, 20) / 10, RND.Next(-20, 20) / 10, 0)
        Entity.Task.DriveTo(Car, DropOffPoint + offsetVector, 1, DrivingSpeed, 1)

    End Sub

End Class

Public Module Globals
    Public DebugMessage As String = ""

    Public ListOfCustomers As New List(Of Customer)

    Public CarTypes() As String = {"adder", "baller", "banshee", "bullet", "carbonizzare", "cavalcade", "cheetah", "cogcabrio", "comet2", "coquette", "dominator", "dubsta", "elegy2", "entityxf", "f620", "felon", "felon2", "feltzer2", "fq2", "fusilade", "futo", "gauntlet", "infernus", "issi2", "khamelion", "manana", "mesa", "monroe", "ninef", "ninef2", "oracle", "oracle2", "patriot", "penumbra", "peyote", "rapidgt", "rapidgt2", "rocoto", "schafter2", "schwarzer", "sentinel", "sentinel2", "serrano", "stinger", "stingergt", "sultan", "superd", "surano", "tailgater", "vacca", "voltic", "zion", "zion2", "ztype", "jester", "turismor", "alpha", "massacro", "zentorno", "huntley", "pigalle", "blade", "coquette2", "furoregt", "casco", "kuruma", "glendale", "warrener", "stalion"}
    Public AmbCarTypes() As String = {"baller", "cavalcade", "coquette", "dominator", "dubsta", "felon", "felon2", "feltzer2", "fq2", "fusilade", "futo", "gauntlet", "issi2", "manana", "mesa", "oracle", "oracle2", "penumbra", "rocoto", "schafter2", "schwarzer", "sentinel", "sentinel2", "serrano", "sultan", "tailgater", "zion", "zion2", "huntley", "kuruma", "glendale", "warrener", "stalion", "asterope", "asea", "bjxl", "blista", "buccaneer", "buffalo", "dilettante", "emperor", "habanero", "ingot", "jackal", "minivan", "phoenix", "prairie", "premier", "primo", "radi", "regina", "stanier", "stratum"}

    Public PedTypes() As String = {"a_f_m_bevhills_01", "a_f_m_bevhills_02", "a_f_m_business_02", "a_f_m_downtown_01", "a_f_m_soucent_01", "a_f_m_soucent_02", "a_f_o_soucent_01", "a_f_o_soucent_02", _
                               "a_f_y_bevhills_01", "a_f_y_bevhills_02", "a_f_y_bevhills_03", "a_f_y_bevhills_04", "a_f_y_business_01", "a_f_y_business_02", "a_f_y_business_03", "a_f_y_business_04", _
                               "a_f_y_genhot_01", "a_f_y_hipster_01", "a_f_y_hipster_02", "a_f_y_hipster_03", "a_f_y_hipster_04", "a_f_y_scdressy_01", "a_f_y_soucent_01", "a_f_y_soucent_02", _
                               "a_f_y_soucent_03", "a_f_y_tourist_01", "a_f_y_tourist_02", "a_f_y_vinewood_01", "a_f_y_vinewood_02", "a_f_y_vinewood_03", "a_f_y_vinewood_04", "a_m_m_bevhills_01",
                               "a_m_m_bevhills_02", "a_m_m_business_01", "a_m_m_malibu_01", "a_m_m_soucent_01", "a_m_m_soucent_02", "a_m_m_soucent_03", "a_m_m_soucent_04", "a_m_m_tourist_01", _
                               "a_m_o_soucent_01", "a_m_o_soucent_02", "a_m_o_soucent_03", "a_m_y_bevhills_01", "a_m_y_bevhills_02", "a_m_y_business_01", "a_m_y_business_02", "a_m_y_business_03", _
                               "a_m_y_downtown_01", "a_m_y_gay_01", "a_m_y_gay_02", "a_m_y_hipster_01", "a_m_y_hipster_02", "a_m_y_hipster_03", "a_m_y_indian_01", "a_m_y_latino_01", "a_m_y_soucent_01", _
                               "a_m_y_soucent_02", "a_m_y_soucent_03", "a_m_y_soucent_04", "a_m_y_vindouche_01", "a_m_y_vinewood_01", "a_m_y_vinewood_02", "a_m_y_vinewood_03", "a_m_y_vinewood_04", _
                               "s_f_y_movprem_01"}
    Public UppercaseLetters() As String = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}

    Public MissionVehicleSpawnTime As Integer = 0
    Public AmbientVehicleSpawnTime As Integer = 0

    'COORDINATES
    Public AlleyEntranceSouth As New Vector3(-1222.2, -272.2, 38.1)
    Public AlleyEntranceSouthHDG As Integer = 27
    Public AlleyEntranceNorth As New Vector3(-1326.8, -83.4, 48.7)
    Public AlleyEntranceNorthHDG As Integer = 4
    Public PedSpawnPoint1 As New Vector3(-1219.4, -266.1, 38.5)

    Public CarSpawnPoint1 As New Vector3(-1222.7, -263.2, 37.9)
    Public CarSpawnPoint1HDG As Integer = 27
    Public CarSpawnPoint2 As New Vector3(-1332.5, -91.5, 49.5)
    Public CarSpawnPoint2HDG As Integer = 184

    'Public DespawnPoint1 As New Vector3(-1326.8, -83.4, 48.7)
    'Public DespawnPoint2 As New Vector3(-1356, -215.1, 43.3)
    'Public DespawnPoint3 As New Vector3(-1272.2, -267.5, 38.7)

    Public GarageEntrance As New Vector3(-1261.9, -241.1, 41.7)
    Public DropOffPoint As New Vector3(-1227.4, -190.7, 39.2)
    Public AmbientDropOffPoint As New Vector3(-1227.6, -189, 38.8)

    Public GarageSpot1 As New Vector3(-1301, -208, 46.5)
    Public GarageSpot1HDG As Integer = 214
End Module



'======================
' AMBIENT EVENTS
'======================
Public Module AmbientEvents
    Public RND As New Random

    Public Car As Vehicle
    Private Ped1, Ped2, Ped3, Ped4 As Ped
    Private AllPeds As New List(Of Ped)

    Public AmbientStage As AmbientStages = AmbientStages.OFF
    Public NextAmbientEventTriggerTime As Integer = 60 * 60 * 1000
    Public Countdown As Integer = 0

    Public GarageLVL1RampBottom As New Vector3(-1276.8, -240.6, 42.3)


    Public Enum AmbientStages
        OFF
        WAITING

        AMB1_0
        AMB1_1_DriveToDespawn

        AMB2_0
        AMB2_1_DriveToGarage

        AMB3_0
        AMB3_1_DriveToDropoff
        AMB3_2_Stopping
        AMB3_3_PedsExiting
        AMB3_4
        AMB3_5

        AMB4_0
        AMB4_1

        AMB5_0
        AMB5_1
        AMB5_2
        AMB5_3
    End Enum

    Public Sub StartAmbientEvents()
        StartWaitingForAmbientEvents()
    End Sub

    Public Sub EndAmbientEvents()
        DismissAllAmbientEntities()
        AmbientStage = AmbientStages.OFF
    End Sub

    Public Sub CreateRandomCarLoad(position As Vector3, minimumNumOfPassengers As Integer, maxNumOfPassengers As Integer, heading As Integer)
        Dim r As Integer = RND.Next(0, AmbCarTypes.Length)
        Car = World.CreateVehicle(AmbCarTypes(r), position)
        Car.Heading = heading

        r = RND.Next(0, PedTypes.Length)
        Ped1 = Car.CreatePedOnSeat(VehicleSeat.Driver, PedTypes(r))
        AllPeds.Add(Ped1)

        Dim max As Integer = GTA.Native.Function.Call(Of Integer)(Native.Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, Car)
        If max > maxNumOfPassengers Then max = maxNumOfPassengers

        If minimumNumOfPassengers > maxNumOfPassengers Then minimumNumOfPassengers = maxNumOfPassengers

        r = RND.Next(minimumNumOfPassengers, max + 1)

        If r > 2 Then
            r = RND.Next(0, PedTypes.Length)
            Ped4 = Car.CreatePedOnSeat(VehicleSeat.LeftRear, PedTypes(r))
            AllPeds.Add(Ped4)
        End If

        If r > 1 Then
            r = RND.Next(0, PedTypes.Length)
            Ped3 = Car.CreatePedOnSeat(VehicleSeat.RightRear, PedTypes(r))
            AllPeds.Add(Ped3)
        End If

        If r > 0 Then
            r = RND.Next(0, PedTypes.Length)
            Ped2 = Car.CreatePedOnSeat(VehicleSeat.RightFront, PedTypes(r))
            AllPeds.Add(Ped2)
        End If

        AmbientVehicleSpawnTime = Game.GameTime
    End Sub

    Public Sub UpdateAmbientEvents()

        checkIfItsTimeToTriggerAnAmbientEvent()

        check_AMB1_0()
        check_AMB1_1()

        check_AMB2_0()
        check_AMB2_1()

        check_AMB3_0()
        check_AMB3_1()
        check_AMB3_2()
        check_AMB3_3()
        check_AMB3_4()
        check_AMB3_5()

        check_AMB4_0()
        check_AMB4_1()

        check_AMB5_0()
        check_AMB5_1()
        check_AMB5_2()
        check_AMB5_3()
    End Sub

    Public Sub DismissAllAmbientEntities()
        If Ped1 IsNot Nothing Then
            If Ped1.Exists Then
                Ped1.MarkAsNoLongerNeeded()
            End If
        End If

        If Ped2 IsNot Nothing Then
            If Ped2.Exists Then
                Ped2.MarkAsNoLongerNeeded()
                If Ped2.IsInVehicle Then
                    Ped2.Delete()
                End If
            End If
        End If

        If Ped3 IsNot Nothing Then
            If Ped3.Exists Then
                Ped3.MarkAsNoLongerNeeded()
                If Ped3.IsInVehicle Then
                    Ped3.Delete()
                End If
            End If
        End If

        If Ped4 IsNot Nothing Then
            If Ped4.Exists Then
                Ped4.MarkAsNoLongerNeeded()
                If Ped4.IsInVehicle Then
                    Ped4.Delete()
                End If
            End If
        End If

        If Car IsNot Nothing Then
            If Car.Exists Then
                Car.MarkAsNoLongerNeeded()
            End If
        End If

        Ped1 = Nothing
        Ped2 = Nothing
        Ped3 = Nothing
        Ped4 = Nothing
        Car = Nothing

    End Sub

    Public Sub StartWaitingForAmbientEvents()
        NextAmbientEventTriggerTime = Game.GameTime + RND.Next(5000, 25000)
        AmbientStage = AmbientStages.WAITING
    End Sub

    Public Sub checkIfItsTimeToTriggerAnAmbientEvent()

        If AmbientStage <> AmbientStages.WAITING Then Exit Sub

        If Game.GameTime < NextAmbientEventTriggerTime Then Exit Sub

        'ALL CONDITIONS MET

        Dim NumOfEvents As Integer = 3
        Dim r As Integer = RND.Next(0, NumOfEvents + 1)

        Select Case r
            Case 0
                AmbientStage = AmbientStages.AMB1_0
            Case 1
                AmbientStage = AmbientStages.AMB4_0
            Case 2
                AmbientStage = AmbientStages.AMB5_0
            Case Else
                AmbientStage = AmbientStages.AMB1_0
        End Select
    End Sub

    'SPAWN POINT 1 TO DESPAWN POINT 1
    Public Sub check_AMB1_0()

        If AmbientStage <> AmbientStages.AMB1_0 Then Exit Sub

        If ValetVCRichman.CurrentCustomer IsNot Nothing Then
            Dim d As Single = ValetVCRichman.CurrentCustomer.Car.Position.DistanceTo(CarSpawnPoint1)
            DebugMessage = "AMB1: Distance to other car: " & Math.Round(d, 1)
            If d < 15 Then Exit Sub
        End If

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(CarSpawnPoint1)
        DebugMessage = "AMB1: Distance to player: " & Math.Round(dist, 1) & " (Min 60)"
        If dist < 60 Then Exit Sub

        'All conditions met

        CreateRandomCarLoad(CarSpawnPoint1, 0, 3, CarSpawnPoint1HDG)

        Ped1.Task.DriveTo(Car, AlleyEntranceNorth, 5, RND.Next(60, 160) / 10, 1)

        AmbientStage = AmbientStages.AMB1_1_DriveToDespawn
    End Sub

    Public Sub check_AMB1_1()
        If AmbientStage <> AmbientStages.AMB1_1_DriveToDespawn Then Exit Sub

        Dim dist As Single
        dist = World.GetDistance(Car.Position, AlleyEntranceNorth)
        DebugMessage = "Dist to despawn: " & Math.Round(dist) & " (TGT 50)"
        If dist > 50 Then Exit Sub

        'All conditions met

        DismissAllAmbientEntities()
        StartWaitingForAmbientEvents()
    End Sub

    ' SPAWN POINT 1 INTO GARAGE
    Public Sub check_AMB2_0()
        If AmbientStage <> AmbientStages.AMB2_0 Then Exit Sub

        If ValetVCRichman.CurrentCustomer IsNot Nothing Then
            Dim d As Single = ValetVCRichman.CurrentCustomer.Car.Position.DistanceTo(CarSpawnPoint1)
            DebugMessage = "AMB2: Distance to other car: " & Math.Round(d, 1)
            If d < 15 Then Exit Sub
        End If

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(CarSpawnPoint1)
        If dist < 66 Then Exit Sub

        'ALL CONDITIONS MET

        CreateRandomCarLoad(CarSpawnPoint1, 0, 0, CarSpawnPoint1HDG)

        Ped1.Task.DriveTo(Car, GarageEntrance, 5, RND.Next(5, 8), 1)

        AmbientStage = AmbientStages.AMB2_1_DriveToGarage
    End Sub

    Public Sub check_AMB2_1()
        If AmbientStage <> AmbientStages.AMB2_1_DriveToGarage Then Exit Sub

        Dim dist As Single
        dist = Car.Position.DistanceTo(GarageEntrance)
        If dist > 10 Then Exit Sub

        'ALL CONDITIONS MET

        DismissAllAmbientEntities()
        StartWaitingForAmbientEvents()
    End Sub

    ' SPAWN POINT 1, DROP OFF AT HOTEL, DESPAWN POINT 1
    ' DON'T USE... CANT GET DRIVERS TO BEHAVE RIGHT NOW
    Public Sub check_AMB3_0()
        If AmbientStage <> AmbientStages.AMB3_0 Then Exit Sub

        If Game.GameTime < MissionVehicleSpawnTime + 10000 Then Exit Sub

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(CarSpawnPoint1)
        If dist < 66 Then Exit Sub

        'ALL CONDITIONS MET

        CreateRandomCarLoad(CarSpawnPoint1, 1, 3, CarSpawnPoint1HDG)

        Dim offsetVector As New Vector3(RND.Next(-10, 10) / 10, RND.Next(-10, 10) / 10, 0)
        Ped1.Task.DriveTo(Car, AmbientDropOffPoint + offsetVector, 0.5, RND.Next(5, 12), 1)

        AmbientStage = AmbientStages.AMB3_1_DriveToDropoff
    End Sub

    Public Sub check_AMB3_1()
        If AmbientStage <> AmbientStages.AMB3_1_DriveToDropoff Then Exit Sub

        Dim dist As Single
        dist = Car.Position.DistanceTo(AmbientDropOffPoint)
        If dist > 4 Then Exit Sub

        'ALL CONDITIONS MET

        AmbientStage = AmbientStages.AMB3_2_Stopping
    End Sub

    Public Sub check_AMB3_2()
        If AmbientStage <> AmbientStages.AMB3_2_Stopping Then Exit Sub

        If Car.Speed > 0.05 Then Exit Sub

        'ALL CONDITIONS MET

        For Each P As Ped In AllPeds
            If P <> Ped1 Then
                Dim exitSeq As New TaskSequence
                exitSeq.AddTask.Wait(RND.Next(100, 1000))
                exitSeq.AddTask.LeaveVehicle(Car, True)
                exitSeq.Close()
                P.Task.PerformSequence(exitSeq)
            End If
        Next
        Ped1.Task.ClearAll()

        AmbientStage = AmbientStages.AMB3_3_PedsExiting
    End Sub

    Public Sub check_AMB3_3()
        If AmbientStage <> AmbientStages.AMB3_3_PedsExiting Then Exit Sub

        For Each P As Ped In AllPeds
            If P <> Ped1 Then
                If P.IsInVehicle = True Then Exit Sub
            End If
        Next

        'ALL CONDITIONS MET

        Car.CloseDoor(VehicleDoor.FrontRightDoor, True)
        Car.CloseDoor(VehicleDoor.BackRightDoor, True)
        Car.CloseDoor(VehicleDoor.BackLeftDoor, True)

        Dim p1 As Vector3 = Car.Position + (Car.ForwardVector * 0.35)
        Dim p2 As New Vector3(-1224.3, -181.1, 39.2)
        Dim p3 As New Vector3(-1227, -174.2, 39.3)
        Dim p4 As New Vector3(-1243.9, -162.4, 40.4)
        Dim p5 As New Vector3(-1228.1, -139.2, 40.4)


        For Each p As Ped In AllPeds
            If p <> Ped1 Then
                Dim pedcar As Vector3 = p.Position - Car.Position

                p1 = p.Position + (Car.ForwardVector * 5) + (p.ForwardVector * 1.2) + pedcar
                Dim route() As Vector3 = {p1, p2, p3, p4, p5}
                p.Task.FollowPointRoute(route)
            End If
        Next

        AmbientStage = AmbientStages.AMB3_4
    End Sub

    Public Sub check_AMB3_4()
        If AmbientStage <> AmbientStages.AMB3_4 Then Exit Sub

        Dim dist As Single
        dist = Ped2.Position.DistanceTo(Car.Position)
        DebugMessage = "Ped dist from car: " & Math.Round(dist)
        If dist < 8 Then Exit Sub

        'ALL CONDITIONS MET

        Ped1.Task.DriveTo(Car, AlleyEntranceNorth, 5, RND.Next(5, 12), 1)

        AmbientStage = AmbientStages.AMB3_5
    End Sub

    Public Sub check_AMB3_5()
        If AmbientStage <> AmbientStages.AMB3_5 Then Exit Sub

        Dim dist As Single
        dist = Car.Position.DistanceTo(AlleyEntranceNorth)
        DebugMessage = "Car dist from despawn pt: " & Math.Round(dist, 1)
        If dist > 70 Then Exit Sub

        'ALL CONDITIONS MET

        DismissAllAmbientEntities()
        StartWaitingForAmbientEvents()
    End Sub

    'DESPAWN POINT 1 TO ALLEY ENTRANCE SOUTH
    Public Sub check_AMB4_0()

        If AmbientStage <> AmbientStages.AMB4_0 Then Exit Sub

        If ValetVCRichman.CurrentCustomer IsNot Nothing Then
            Dim dist As Single = ValetVCRichman.CurrentCustomer.Car.Position.DistanceTo(CarSpawnPoint2)
            DebugMessage = "AMB4_0: " & dist
            If dist < 15 Then Exit Sub
        End If

        'All conditions met

        CreateRandomCarLoad(CarSpawnPoint2, 0, 3, CarSpawnPoint2HDG)

        Ped1.Task.DriveTo(Car, AlleyEntranceSouth, 5, RND.Next(60, 150) / 10, 1)

        AmbientStage = AmbientStages.AMB4_1
    End Sub

    Public Sub check_AMB4_1()
        If AmbientStage <> AmbientStages.AMB4_1 Then Exit Sub

        If Car.IsOnAllWheels = False Then
            Car.PlaceOnGround()
            Car.Repair()
        End If

        Dim dist As Single
        dist = World.GetDistance(Car.Position, AlleyEntranceSouth)
        DebugMessage = "Dist to endpoint: " & Math.Round(dist) & " (TGT 10)"
        If dist > 10 Then Exit Sub

        'All conditions met

        DismissAllAmbientEntities()
        StartWaitingForAmbientEvents()
    End Sub

    'GARAGE SPOT 1 TO Despawn Point 2
    Public Sub check_AMB5_0()

        If AmbientStage <> AmbientStages.AMB5_0 Then Exit Sub

        Dim dist As Single = Game.Player.Character.Position.DistanceTo(GarageSpot1)
        DebugMessage = "AMB5_0: dist " & Math.Round(dist, 1) & " TGT 30"
        If dist < 30 Then Exit Sub

        'All conditions met

        CreateRandomCarLoad(GarageSpot1, 0, 3, GarageSpot1HDG)

        Ped1.Task.DriveTo(Car, GarageLVL1RampBottom, 0, RND.Next(25, 40) / 10, DrivingStyle.Rushed)

        AmbientStage = AmbientStages.AMB5_1
    End Sub

    Public Sub check_AMB5_1()
        If AmbientStage <> AmbientStages.AMB5_1 Then Exit Sub

        Dim dist As Single
        dist = World.GetDistance(Car.Position, GarageLVL1RampBottom)
        DebugMessage = "Dist to endpoint: " & Math.Round(dist, 2) & " (TGT 0.75) / v: " & Math.Round(Car.Speed, 2)
        If dist > 0.75 Then Exit Sub

        'All conditions met

        Ped1.Task.DriveTo(Car, GarageEntrance, 0, RND.Next(25, 40) / 10, DrivingStyle.Rushed)
        AmbientStage = AmbientStages.AMB5_2
    End Sub

    Public Sub check_AMB5_2()
        If AmbientStage <> AmbientStages.AMB5_2 Then Exit Sub

        Dim dist As Single
        dist = World.GetDistance(Car.Position, GarageEntrance)
        DebugMessage = "Dist to endpoint: " & Math.Round(dist, 1) & " (TGT 5) / v: " & Math.Round(Car.Speed, 2)
        If dist > 5 Then Exit Sub

        'All conditions met

        Ped1.Task.DriveTo(Car, AlleyEntranceNorth, 1, RND.Next(50, 140) / 10, DrivingStyle.Rushed)
        AmbientStage = AmbientStages.AMB5_3
    End Sub

    Public Sub check_AMB5_3()
        If AmbientStage <> AmbientStages.AMB5_3 Then Exit Sub

        Dim dist As Single
        dist = World.GetDistance(Car.Position, AlleyEntranceNorth)
        DebugMessage = "Dist to endpoint: " & Math.Round(dist) & " (TGT 70) / v: " & Math.Round(Car.Speed, 2)
        If dist > 70 Then Exit Sub

        'All conditions met

        DismissAllAmbientEntities()
        StartWaitingForAmbientEvents()
    End Sub

End Module

' TO DO

' 1) Convertible Roof-Down requests should only be made in good weather.
'    Wait for readable variable through scripthookdotnet

' 2) Ambient activity around the area.
'    - Cars entering/leaving parking deck
'    - Other cars driving up and picking people up/dropping people off.
'    - People walking by
'    - Car thief?

' 3) Wait times differ during different times of the day / days of the week

' 4) Change implementation of car list
'    - include full make/model string for use in case customer forgot valet stub

' 5) Clear area around spawn point for existing vehicles before spawning another.

' 6) Peds follow point array to holding point instead of running