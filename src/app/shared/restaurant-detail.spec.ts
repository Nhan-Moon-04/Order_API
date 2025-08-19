import { TestBed } from '@angular/core/testing';

import { RestaurantDetail } from './restaurant-detail';

describe('RestaurantDetail', () => {
  let service: RestaurantDetail;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RestaurantDetail);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
