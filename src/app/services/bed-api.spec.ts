import { TestBed } from '@angular/core/testing';

import { BedApi } from './bed-api';

describe('BedApi', () => {
  let service: BedApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BedApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
