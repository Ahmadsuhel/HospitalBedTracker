import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HospitalList } from './hospital-list';

describe('HospitalList', () => {
  let component: HospitalList;
  let fixture: ComponentFixture<HospitalList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HospitalList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HospitalList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
