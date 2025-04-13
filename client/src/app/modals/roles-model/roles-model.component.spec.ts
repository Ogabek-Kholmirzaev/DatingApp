import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolesModelComponent } from './roles-model.component';

describe('RolesModelComponent', () => {
  let component: RolesModelComponent;
  let fixture: ComponentFixture<RolesModelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RolesModelComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(RolesModelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
