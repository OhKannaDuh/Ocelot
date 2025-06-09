using System;
using System.Collections.Generic;
using System.Numerics;
using Ocelot.Chain;
using Ocelot.IPC;
using Ocelot.Prowler;

namespace OccultCrescentHelper.Chains;

public class ProwlerChain : ChainFactory
{
    private VNavmesh vnav;

    private Func<Vector3, List<IProwlerAction>> factory;

    private Vector3 destination;

    public ProwlerChain(VNavmesh vnav, Func<Vector3, List<IProwlerAction>> factory, Vector3 destination)
    {
        this.vnav = vnav;
        this.factory = factory;
        this.destination = destination;
    }

    protected override Chain Create(Chain chain)
    {
        return chain.Then(Prowler.Create(new(vnav), factory(destination)));
    }
}
