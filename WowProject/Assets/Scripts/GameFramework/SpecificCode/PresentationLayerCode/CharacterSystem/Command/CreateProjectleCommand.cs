using UnityEngine;
/// <summary>
/// 创建人：Trigger 
/// 命令名称：
/// 参数:CreateProjectleCommandParam
/// </summary>
public struct CreateProjectleCommand : ICommand
{
    public void Execute(object dataObj)
    {
        CreateProjectleCommandParam cpcp = (CreateProjectleCommandParam)dataObj;
        Bullet b = GameObject.Instantiate(GameResSystem.GetRes<GameObject>
                ("Prefabs/Projectile/Bullet"),
                cpcp.pos+new Vector3(0,1,0), Quaternion.identity).GetComponent<Bullet>();
        b.targetTrans = cpcp.t;
        ROLE r = this.GetSystem<INetSystem>().GetPSDValue().role;
        if (r==ROLE.HUNTER)
        {
            b.speed = 20;
        }
        else
        {
            b.speed = 10;
        }
    }
}

public struct CreateProjectleCommandParam
{
    public Transform t;
    public Vector3 pos;
}
