import { TestBed } from '@angular/core/testing';

import { RouteResolverMemberDetailResolver } from './route-resolver-member-detail.resolver';

describe('RouteResolverMemberDetailResolver', () => {
  let resolver: RouteResolverMemberDetailResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    resolver = TestBed.inject(RouteResolverMemberDetailResolver);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });
});
