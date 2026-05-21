import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BedCard } from './bed-card';

describe('BedCard', () => {
  let component: BedCard;
  let fixture: ComponentFixture<BedCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BedCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BedCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
