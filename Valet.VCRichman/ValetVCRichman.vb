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
    Public RND As New Random

    Public isMinigameActive As Boolean = False
    Public CurrentMinigameStage As MiniGameStages = MiniGameStages.OFF

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
    Public HoldingPosition As New Vector3(-1239.6, -156.26, 40.41)
    Public WaitingPosition As New Vector3(-1212.4, -186.3, 39.3)
    Public WaitingHeading As Integer = 105
    Public DespawnPoint1 As New Vector3(-1326.8, -83.4, 48.7)
    Public DespawnPoint2 As New Vector3(-1356, -215.1, 43.3)
    Public DespawnPoint3 As New Vector3(-1272.2, -267.5, 38.7)

    Public ValetPed As Ped
    Public ValetPedSpawnPoint As New Vector3(-1246.3, -219.8, 40.7)
    Public ValetPedSpawnHeading As Integer = 204

    Public GarageEntrance As New Vector3(-1261.9, -241.1, 41.7)
    Public GarageBlip As Blip

    Public UI_Trigger_width As Integer = 280
    Public UI_Trigger_height As Integer = 35
    Public UI_Trigger_x As Integer = (1280 / 2) - (UI_Trigger_width / 2)
    Public UI_Trigger_y As Integer = (720 / 2) - (UI_Trigger_height / 2)
    Public UI_Trigger As New UIContainer(New Point(UI_Trigger_x, UI_Trigger_y), New Size(UI_Trigger_width, UI_Trigger_height), Color.FromArgb(0, 0, 0, 0)) With {.Enabled = False}

    Public UI_Debug_width As Integer = 180
    Public UI_Debug As New UIContainer(New Point(1280 - UI_Debug_width, 180), New Size(UI_Debug_width, 120), Color.FromArgb(170, 0, 0, 0))

    Public CurrentCustomer As Customer

    Public EntourageGroup As Integer

    Public IN_5_Timer As Integer = 0
    Public OUT_2_Timer As Integer = 0
    Public OUT_9_Timer As Integer = 0
    Public BACK_2_Timer As Integer = 0

    Public Enum MiniGameStages
        OFF
        WaitingForNextTask

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

    Public Enum AmbientStages
        OFF
        WAITING
    End Enum

    Public Sub New()
        CurrentMinigameStage = MiniGameStages.OFF

        EntourageGroup = World.AddRelationShipGroup("Entourage")

        clearAllActiveBlips()
    End Sub

    Public Sub clearAllActiveBlips()
        Dim ActiveBlips() As Blip
        ActiveBlips = World.GetActiveBlips

        For Each b As Blip In ActiveBlips
            b.Remove()
        Next
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

        checkIfPlayerIsAtTriggerLocation()
        checkIfPlayerIsWithinPlayingArea()
        checkIfPlayerIsAlive()
        checkIfPlayerIsWanted()

        checkIfItsTimeToTriggerTheNextEvent()

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

        'isItTimeToForgetTheCustomer()

        drawUI()
    End Sub

    Public Sub checkIfPlayerIsAtTriggerLocation()
        Dim ppos As Vector3 = Game.Player.Character.Position
        Dim dist As Single = World.GetDistance(ppos, TriggerLocation)

        If dist < 1.5 Then
            If Game.Player.Character.Heading > TriggerHeading - 25 And Game.Player.Character.Heading < TriggerHeading + 25 Then
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
        CurrentCustomer = Nothing
        CurrentCustomer = New Customer
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
        If dist > 3 Then Exit Sub

        ' ALL CONDITIONS MET

        CurrentCustomer.Entity.Task.LeaveVehicle(CurrentCustomer.Car, False)

        If CurrentCustomer.Entourage.Count > 0 Then
            For Each p As Ped In CurrentCustomer.Entourage
                p.Task.LeaveVehicle(CurrentCustomer.Car, True)
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

        CurrentCustomer.Entity.Task.GoTo(HoldingPosition)
        GTA.UI.ShowSubtitle("Get in the customer's ~g~car~w~.")

        If CurrentCustomer.Entourage.Count > 0 Then
            For Each p As Ped In CurrentCustomer.Entourage
                p.Task.GoTo(HoldingPosition)
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
            Next
            CurrentCustomer.Entourage = Nothing
        End If

        CurrentCustomer.Car.PreviouslyOwnedByPlayer = True
        CurrentCustomer.Entity.Task.WanderAround(HoldingPosition, 3)

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

        NextTaskTriggerTime = Game.GameTime + RND.Next(3000, 5000)
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

        If numOfPax > 2 Then
            Dim p3 As Ped
            p3 = World.CreateRandomPed(HoldingPosition)
            CurrentCustomer.Entourage.Add(p3)
            p3.RelationshipGroup = EntourageGroup
        End If

        If numOfPax > 1 Then
            Dim p2 As Ped
            p2 = World.CreateRandomPed(HoldingPosition)
            CurrentCustomer.Entourage.Add(p2)
            p2.RelationshipGroup = EntourageGroup
        End If

        If numOfPax > 0 Then
            Dim p1 As Ped
            p1 = World.CreateRandomPed(HoldingPosition)
            CurrentCustomer.Entourage.Add(p1)
            p1.RelationshipGroup = EntourageGroup
        End If

        

        CurrentCustomer.Entity.Task.GoTo(Game.Player.Character)

        If CurrentCustomer.Entourage IsNot Nothing Then
            For Each p As Ped In CurrentCustomer.Entourage
                p.Task.GoTo(CurrentCustomer.Entity)
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

        Dim stub As Integer = CurrentCustomer.ValetStub
        Dim txt As String = ""
        Dim r As Integer = RND.Next(0, 7)
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
            Case Else
                txt = "Number " & stub & ", please."
        End Select

        If CurrentCustomer.Car.IsConvertible = True Then ' And World.Weather <> Weather.Raining And World.Weather <> Weather.ThunderStorm Then
            r = RND.Next(0, 2)
            If r > 0 Then
                CurrentCustomer.WantsTopDown = True
                txt = txt & " And can you put the top down, too?"
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

        CurrentCustomer.Entity.Task.GoTo(WaitingPosition, False)

        If CurrentCustomer.Entourage IsNot Nothing Then
            For Each p As Ped In CurrentCustomer.Entourage
                p.Task.GoTo(CurrentCustomer.Entity)
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

        GTA.UI.ShowSubtitle("Drive the car to the ~b~customer~w~.", MessageDisplayTime)
        CurrentCustomer.EntityBlip.IsFlashing = True
        CurrentCustomer.Entity.Heading = 105

        CurrentMinigameStage = MiniGameStages.OUT_5
    End Sub

    'If player has arrived at rotunda
    'Wait for player to get out of the car
    Public Sub check_OUT5_Conditions()
        If CurrentMinigameStage <> MiniGameStages.OUT_5 Then Exit Sub

        Dim dist As Single
        dist = Game.Player.Character.Position.DistanceTo(DropOffPoint)
        If dist > 5 Then Exit Sub

        ' All Conditions Met

        GTA.UI.ShowSubtitle("Exit the car.", MessageDisplayTime)
        CurrentCustomer.EntityBlip.IsFlashing = False

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

        Dim tip As Integer
        If CurrentCustomer.Car.Health < CurrentCustomer.CarHealthReceived Or CurrentCustomer.Car.EngineHealth < CurrentCustomer.EngineHealthReceived Or CurrentCustomer.Car.PetrolTankHealth < CurrentCustomer.TankHealthReceived Then
            GTA.UI.ShowSubtitle("You scratched it! How dare you!", MessageDisplayTime)
            tip = 0
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
            GTA.UI.ShowSubtitle("Thanks.", MessageDisplayTime)
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

        CurrentCustomer.Entity.Task.DriveTo(CurrentCustomer.Car, DespawnPoint1, 5, CurrentCustomer.DrivingSpeed, 1)

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

        NextTaskTriggerTime = Game.GameTime + RND.Next(3000, 5000)
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
    End Sub

    Public Sub TerminateMinigame(Optional condition As String = "")
        Select Case condition
            Case "leftarea"
                GTA.UI.Notify("Valet missions terminated. You left the area")
            Case "joyride"
                GTA.UI.Notify("The hotel called the cops because you stole a customer's vehicle!")
                Game.Player.WantedLevel = 2
                Game.Player.Character.CurrentVehicle.IsWanted = True
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

        End Select

        isMinigameActive = False
        NumOfMistakes = 0
        DismissAllEntities()
        CurrentMinigameStage = MiniGameStages.OFF
    End Sub

    Public Sub DismissAllEntities()

        If ListOfCustomers IsNot Nothing Then
            If ListOfCustomers.Count > 0 Then
                For Each c As Customer In ListOfCustomers

                    If c.Entity IsNot Nothing And c.Entity.Exists Then

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

                        If c.Entourage IsNot Nothing Then
                            If c.Entourage.Count > 0 Then
                                For Each p As Ped In c.Entourage
                                    If p.CurrentBlip.Exists Then
                                        p.CurrentBlip.Remove()
                                    End If
                                    p.MarkAsNoLongerNeeded()
                                Next
                            End If
                        End If

                        c.Entity.MarkAsNoLongerNeeded()
                    End If

                Next
            End If
        End If

        If GarageBlip IsNot Nothing Then
            If GarageBlip.Exists Then
                GarageBlip.Remove()
            End If
        End If

        If ValetPed IsNot Nothing Then
            If ValetPed.Exists Then
                ValetPed.MarkAsNoLongerNeeded()
            End If
        End If

        ListOfCustomers.Clear()
        CurrentCustomer = Nothing
    End Sub




    '==================================================================================================
    'TEST & DEBUG METHODS

    Public Sub DismissAllKeyChecker(ByVal sender As Object, ByVal k As KeyEventArgs) Handles MyBase.KeyUp
        If k.KeyCode <> Keys.K Then Exit Sub
        DismissAllEntities()
    End Sub

    Public Sub drawUI_Debug()
        UI_Debug.Items.Clear()
        Dim row As Integer = -1
        Dim txt As String

        row += 1
        UI_Debug.Items.Add(New UIText(DebugMessage, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Active? " & isMinigameActive.ToString
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Trigger Dist: " & World.GetDistance(Game.Player.Character.Position, TriggerLocation)
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

        row += 1
        If Game.Player.Character.IsInVehicle Then
            For Each c As Customer In ListOfCustomers
                If c.Car = Game.Player.Character.CurrentVehicle Then
                    txt = "Valet Stub: " & c.ValetStub
                End If
            Next
        Else
            txt = "Valet Stub: not in car"
        End If
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        row += 1
        txt = "Total Customers: " & ListOfCustomers.Count
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        'GoTo Skipr

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

Skipr:
        row += 1
        txt = "Player in car? " & Game.Player.Character.IsInVehicle.ToString
        UI_Debug.Items.Add(New UIText(txt, New Point(0, 10 * row), 0.2, Color.White, 0, False))

        UI_Debug.Draw()
    End Sub
End Class


' CUSTOMERS

Public Class Customer
    Public RND As New Random

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

    Public ValetStub As Integer = 0
    Public Entourage As New List(Of Ped)

    Public CarSpawn1 As New Vector3(-1222.2, -272.2, 38.1) ' SERVICE LOT: (-1149.1, -221.6, 37.2)
    Public CarHeading1 As Integer = 20 ' SERVICE LOT: 202
    Public PedSpawn1 As New Vector3(-1219.4, -266.1, 38.5) 'SERVICE LOT: (-1145.4, -219.9, 37.9)

    Public Sub New()

        ListOfCustomers.Add(Me)

        'GTA.Native.Function.Call(Native.Hash.CLEAR_AREA, CarSpawn1.X, CarSpawn1.Y, CarSpawn1.Z, 25, False)
        'GTA.Native.Function.Call(Native.Hash.CLEAR_AREA_OF_PEDS, PedSpawn1.X, PedSpawn1.Y, PedSpawn1.Z, 25, False)
        'GTA.Native.Function.Call(Native.Hash.CLEAR_AREA_OF_VEHICLES, CarSpawn1.X, CarSpawn1.Y, CarSpawn1.Z, 25, False)

        Dim i As Integer = RND.Next(0, CarTypes.Length)
        Car = World.CreateVehicle(CarTypes(i), CarSpawn1, CarHeading1)
        Car.IsStolen = False
        Car.IsWanted = False
        Dim plate As String = RND.Next(0, 10) & RND.Next(0, 10) & UppercaseLetters(RND.Next(0, UppercaseLetters.Length + 1)) & UppercaseLetters(RND.Next(0, UppercaseLetters.Length + 1)) & UppercaseLetters(RND.Next(0, UppercaseLetters.Length + 1)) & RND.Next(0, 10) & RND.Next(0, 10) & RND.Next(0, 10)
        Car.NumberPlate = plate

        Entity = Car.CreateRandomPedOnSeat(VehicleSeat.Driver) 'World.CreateRandomPed(PedSpawn1) 'GTA.Native.Function.Call(Of Ped)(Native.Hash.CREATE_RANDOM_PED, PedSpawn1.X, PedSpawn1.Y, PedSpawn1.Z)
        Entity.Money = RND.Next(20, 200)

        'Exit Sub

        Dim numOfSeats As Integer = GTA.Native.Function.Call(Of Integer)(Native.Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, Car)
        Dim numOfPax As Integer = RND.Next(0, numOfSeats + 1)

        If numOfPax > 2 Then
            Dim p3 As Ped
            p3 = Car.CreateRandomPedOnSeat(VehicleSeat.RightFront)
            Entourage.Add(p3)
        End If

        If numOfPax > 1 Then
            Dim p2 As Ped
            p2 = Car.CreateRandomPedOnSeat(VehicleSeat.RightFront)
            Entourage.Add(p2)
        End If

        If numOfPax > 0 Then
            Dim p1 As Ped
            p1 = Car.CreateRandomPedOnSeat(VehicleSeat.RightFront)
            Entourage.Add(p1)
        End If

        Entity.Task.DriveTo(Car, DropOffPoint, 1, DrivingSpeed, 1)

    End Sub

End Class

Public Module Customers
    Public ListOfCustomers As New List(Of Customer)
    Public CarTypes() As String = {"adder", "baller", "banshee", "bullet", "carbonizzare", "cavalcade", "cheetah", "cogcabrio", "comet2", "coquette", "dominator", "dubsta", "elegy2", "entityxf", "f620", "felon", "felon2", "feltzer2", "fq2", "gauntlet", "infernus", "issi2", "khamelion", "mesa", "monroe", "ninef", "ninef2", "oracle", "oracle2", "patriot", "peyote", "rapidgt", "rapidgt2", "rocoto", "schafter2", "schwarzer", "sentinel", "sentinel2", "serrano", "stinger", "stingergt", "sultan", "superd", "surano", "tailgater", "vacca", "voltic", "zion", "zion2", "ztype", "jester", "turismor", "alpha", "massacro", "zentorno", "huntley", "pigalle", "coquette2", "furoregt", "casco", "kuruma"}
    Public DropOffPoint As New Vector3(-1228.5, -192.6, 38.5)
    Public DebugMessage As String = ""
    Public UppercaseLetters() As String = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}
End Module



' TO DO

' 1) Check if any of the active customers are dead. If so:
'    - Check if they died on their own. Remove from ListOfCustomers and get on with life
'    - Check if they were killed by player. If yes, quit mission.

' 2) Convertible Roof-Down requests should only be made in good weather.
'    Wait for readable variable through scripthookdotnet

' 3) Ambient activity around the area.
'    - Cars entering/leaving parking deck
'    - Other cars driving up and picking people up/dropping people off.
'    - People walking by

' 4) Wait times differ during different times of the day / days of the week

' 5) Change implementation of car list
'    - include full make/model string for use in case customer forgot valet stub

' 6) Spawn only rich peds instead of random ones

' 7) Clear area around spawn point for existing vehicles before spawning another.

' 8) Peds follow point array to holding point instead of running