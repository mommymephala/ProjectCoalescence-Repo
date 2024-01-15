public class SpawnableEnemyAI : BaseEnemyAI
{
    protected override void SpawnBehavior()
    {
        if (isSpawn)
        {
            _animator.SetTrigger("SpawnTrigger");
            currentState = State.InitialSpawn;
        }
        else
        {
            currentState = State.Idling;
        }
    }
    protected override void Start()
    {
        base.Start();
        if (isSpawn)
        {
            currentState = State.SpawnStall;
        }
        
        else
        {
            currentState = State.Idling;
        }
    }

    protected override void Update()
    {
        base.Update();

        _animator.SetBool("IsSpawn", isSpawn);
    }
    private void EndSpawnAnimation()
    {
        isSpawn = false;
        _animator.SetBool("IsSpawn", isSpawn);
        currentState = State.Idling;
        _agent.isStopped = false;
        _collider.enabled = true;
    }
}
