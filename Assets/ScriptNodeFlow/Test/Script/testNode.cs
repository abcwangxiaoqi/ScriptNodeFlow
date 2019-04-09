using ScriptNodeFlow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enity1 : Node
{
    public Enity1(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity1");

        //get share data and you can modify it
        (shareData as testShareData).state = 3;

        //call finish method when you're sure finished completely        
        finish(true);
    }
}

[BindingFlow(10234)]
[BindingNode("19040820212371")]
public class Enity6 : Node
{
    public Enity6(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity6");

        (shareData as testShareData).state = 3;
        finish(true);
    }
}

public class Enity2 : Node
{
    public Enity2(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity2");
        (shareData as testShareData).state = 10;
        finish(true);
    }
}

public class Enity3 : Node
{
    public Enity3(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity3");
        (shareData as testShareData).state = 20;
        finish(true);
    }
}

public class Enity4 : Node
{
    public Enity4(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity4");
        (shareData as testShareData).state = 30;
        finish(true);
        
    }
}




//////====================================
public class Enity10 : Node
{
    public Enity10(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity10");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity11 : Node
{
    public Enity11(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity11");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity12 : Node
{
    public Enity12(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity12");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity13 : Node
{
    public Enity13(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity13");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity14 : Node
{
    public Enity14(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity14");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity15 : Node
{
    public Enity15(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity15");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity16 : Node
{
    public Enity16(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity16");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity17 : Node
{
    public Enity17(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity17");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity18 : Node
{
    public Enity18(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity18");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity19 : Node
{
    public Enity19(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity19");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity20 : Node
{
    public Enity20(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity20");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity21 : Node
{
    public Enity21(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity21");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity22 : Node
{
    public Enity22(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity22");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity23 : Node
{
    public Enity23(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity23");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity24 : Node
{
    public Enity24(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity24");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity25 : Node
{
    public Enity25(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity25");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity26 : Node
{
    public Enity26(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity26");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity27 : Node
{
    public Enity27(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity27");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity28 : Node
{
    public Enity28(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity28");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity29 : Node
{
    public Enity29(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity29");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity30 : Node
{
    public Enity30(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity30");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity31 : Node
{
    public Enity31(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity31");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity32 : Node
{
    public Enity32(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity32");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity33 : Node
{
    public Enity33(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity33");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity34 : Node
{
    public Enity34(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity34");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity35 : Node
{
    public Enity35(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity35");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity36 : Node
{
    public Enity36(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity36");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity37 : Node
{
    public Enity37(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity37");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity38 : Node
{
    public Enity38(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity38");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity39 : Node
{
    public Enity39(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity39");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}
public class Enity40 : Node
{
    public Enity40(SharedData data) : base(data) { }

    public override void execute()
    {
        Debug.Log("Enity40");
        (shareData as testShareData).state = 30;
        finish(true);

    }
}

