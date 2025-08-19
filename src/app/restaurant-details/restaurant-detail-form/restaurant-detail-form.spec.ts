import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RestaurantDetailForm } from './restaurant-detail-form';

describe('RestaurantDetailForm', () => {
  let component: RestaurantDetailForm;
  let fixture: ComponentFixture<RestaurantDetailForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RestaurantDetailForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RestaurantDetailForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
