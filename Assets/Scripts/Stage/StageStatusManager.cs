﻿using UnityEngine;
using System;
using System.Collections;
using Model;

public class StageStatusManager : MonoBehaviour, IEventDisposable
{
    public event Action<StageStatus> OnStatusChanged = delegate { };
    public event Action<int> OnElapsedDayChanged = delegate { };
    public event Action<Direction> OnStageDirectionChanged = delegate { };

    private int maximumDayLimit;
    private int elapsedDays;
    private StageStatus currentStatus;
    private Direction stageDirection;
    private UnitManager unitManager;

    public Direction StageDirection
    {
        get
        {
            return stageDirection;
        }
        set
        {
            stageDirection = value;
            OnStageDirectionChanged(stageDirection);
        }
    }

    public void InitializeStageStatus(int maximumDayLimit, UnitManager unitManager)
    {
        CommonLogger.Log("StageStatusManager::InitializeStageStatus => 초기화 시작");

        OnElapsedDayChanged += SetToGameOverIfDayExceeded;

        unitManager.OnTurnChanged += SetToGameOverIfDayIsEqualToLimitAndTurnChangedToBoss;
        unitManager.OnTurnChanged += IncreaseDayIfTurnChangedToPlayer;
        this.maximumDayLimit = maximumDayLimit;
        this.unitManager = unitManager;

        StageDirection = Direction.Right;
        CurrentStatus = StageStatus.InProgress;

        ElapsedDays = 0;

        CommonLogger.Log("StageStatusManager::InitializeStageStatus => 초기화 완료");
    }

    public void RegisterEventAfterInit(UnitManager unitManager)
    {
        unitManager.Boss.OnDeath += SetToStageClear;
    }

    private void SetToGameOverIfDayExceeded(int currentDays)
    {
        if (currentDays > maximumDayLimit)
        {
            CommonLogger.LogFormat("StageStatusManager::SetToGameOverIfDayExceeded => 진행 일시가 {0}일을 초과함. 게임 오버!", maximumDayLimit);
            CurrentStatus = StageStatus.Failure;
            StageManager.Instance.StageUi.TransitionToFailure();
        }
    }

    private void SetToGameOverIfDayIsEqualToLimitAndTurnChangedToBoss(TurnState turn)
    {
        if (turn == TurnState.Boss && ElapsedDays == maximumDayLimit)
        {
            CommonLogger.LogFormat("StageStatusManager::SetToGameOverIfDayExceeded => 진행 일시가 최대 제한 일수인 {0}일과 같은 상태에서 플레이어 턴이 종료됨. 게임 오버!", ElapsedDays);
            CurrentStatus = StageStatus.Failure;
            StageManager.Instance.StageUi.TransitionToFailure();
        }
    }

    private void SetToStageClear()
    {
        CurrentStatus = StageStatus.Victory;
        StageManager.Instance.StageUi.TransitionToVictory();
    }

    public StageStatus CurrentStatus
    {
        get
        {
            return currentStatus;
        }
        set
        {
            CommonLogger.LogFormat("StageStatusManager::CurrentStatus => 스테이지 상태가 '{0}'으로 바뀌려 함.", value);
            currentStatus = value;
            OnStatusChanged(currentStatus);
        }
    }

    private void IncreaseDayIfTurnChangedToPlayer(TurnState turn)
    {
        if (turn == TurnState.Player)
        {
            ElapsedDays += 1;
        }
    }

    public int ElapsedDays
    {
        get
        {
            return elapsedDays;
        }
        set
        {
            CommonLogger.LogFormat("StageStatusManager::ElapsedDays => 진행 일시가 {0}으로 바뀌려 함.", value);

            elapsedDays = value;
            OnElapsedDayChanged(elapsedDays);
        }
    }

    public void DisposeRegisteredEvents()
    {
        OnStatusChanged = delegate { };
        OnElapsedDayChanged = delegate { };
        OnStageDirectionChanged = delegate { };

        CommonLogger.Log("StageStatusManager::DisposeRegisteredEvents => 이벤트 Disposing 완료.");
    }
}