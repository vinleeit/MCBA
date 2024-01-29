using Autofac;
using Mcba.Services.Interfaces;

public class McbaModule : Module {
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);


        builder.RegisterType<IDepositService>();
    }
}